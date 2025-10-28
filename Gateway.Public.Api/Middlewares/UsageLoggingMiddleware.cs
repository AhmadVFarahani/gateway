using Gateway.Application.Interfaces;
using Gateway.Application.Usage;
using Gateway.Domain.Entities;
using System.Diagnostics;

namespace Gateway.Public.Api.Middlewares;
/// <summary>
/// Middleware that records request metadata (CompanyId, ContractId, PlanId, RouteId, Duration, StatusCode)
/// and enqueues it for background database persistence.
/// </summary>
public class UsageLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UsageLoggingMiddleware> _logger;
    private readonly IUsageLogQueueService _queue;

    public UsageLoggingMiddleware(RequestDelegate next, ILogger<UsageLoggingMiddleware> logger, IUsageLogQueueService queue)
    {
        _next = next;
        _logger = logger;
        _queue = queue;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip auth for Prometheus metrics and internal endpoint
        if (InternalRequestHelper.IsInternal(context))
        {
            await _next(context);
            return;
        }

        // Start measuring request duration
        var stopwatch = Stopwatch.StartNew();

        // Safe extraction of context items (set by previous middlewares)
        var companyId = GetLongValue(context, "CompanyId");
        var contractId = GetLongValue(context, "ContractId");
        var planId = GetLongValue(context, "PlanId");
        var routeId = GetLongValue(context, "RouteId");
        var userId = GetLongValue(context, "UserId");
        var keyId = GetLongValue(context, "KeyId");

        ClearContextKeys(context, "CompanyId", "ContractId", "PlanId", "RouteId", "UserId", "KeyId");
        try
        {
            await _next(context); // Let the pipeline execute (YARP, controllers, etc.)
        }
        finally
        {
            stopwatch.Stop();

            try
            {
            
                // Build log entry
                var logEvent = new UsageLog
                {
                    UserId = userId,
                    KeyId = keyId,
                    CompanyId = companyId,
                    ContractId = contractId,
                    PlanId = planId,
                    RouteId = routeId,
                    ResponseStatusCode = context.Response?.StatusCode ?? 0,
                    DurationMs = stopwatch.ElapsedMilliseconds,
                };

                // Enqueue for background batch insert
                await _queue.EnqueueAsync(logEvent);

                _logger.LogDebug(
                    "Usage log captured → CompanyId={CompanyId}, ContractId={ContractId}, PlanId={PlanId}, RouteId={RouteId}, Status={Status}, Duration={Duration}ms",
                    companyId, contractId, planId, routeId, logEvent.ResponseStatusCode, logEvent.DurationMs);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to enqueue usage log event.");
            }
        }
    }

    /// <summary>
    /// Safely retrieves a long value from HttpContext.Items.
    /// Returns 0 if not found or invalid.
    /// </summary>
    private static long GetLongValue(HttpContext context, string key)
    {
        if (context.Items.TryGetValue(key, out var value) && value != null)
        {
            try
            {
                return Convert.ToInt64(value);
            }
            catch
            {
                return 0;
            }
        }
        return 0;
    }

    private static void ClearContextKeys(HttpContext context, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (context.Items.ContainsKey(key))
                context.Items.Remove(key);
        }
    }
}
