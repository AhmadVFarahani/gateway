using Gateway.Application.Contract.Dtos;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.AspNetCore.Routing;
using Gateway.Application.Routes.Dtos;

namespace Gateway.Public.Api.Middlewares;

public class PlanValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PlanValidationMiddleware> _logger;
    private readonly IConnectionMultiplexer _redis;

    // Redis key patterns (adjust if your keys differ)
    private static string ContractKeyPattern(long companyId) => $"contract:{companyId}:*";
    private static string PlanKey(long planId) => $"plan:{planId}";
    private static string PlanRouteKey(long planId, long routeId) => $"planroute:{planId}:{routeId}";

    public PlanValidationMiddleware(
        RequestDelegate next,
        ILogger<PlanValidationMiddleware> logger,
        IConnectionMultiplexer redis)
    {
        _next = next;
        _logger = logger;
        _redis = redis;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1) Must be authenticated (previous middleware should have validated JWT)
        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        // 2) Extract required claims (UserId, CompanyId)
        if (!long.TryParse(context.User.FindFirst("UserId")?.Value, out var userId) || userId <= 0)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Error Code 20001 - Missing UserId");
            return;
        }

        if (!long.TryParse(context.User.FindFirst("CompanyId")?.Value, out var companyId) || companyId <= 0)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Error Code 20002 - Missing CompanyId");
            return;
        }

        // 3) Get the resolved Route (set by AccessAuthorizationMiddleware)
        if (!context.Items.TryGetValue("RouteInfo", out var routeObj) || routeObj is not RouteDto route)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Error Code 20003 - Route context not found");
            return;
        }

        var db = _redis.GetDatabase();
        var requestPath = context.Request.Path.Value?.TrimEnd('/').ToLowerInvariant() ?? "";

        // 4) Enumerate company contracts via SCAN (server.Keys)
        //    Pick a server endpoint (prefer a primary/writable endpoint)
        var server = GetReadableServer(_redis);
        if (server is null)
        {
            _logger.LogError("No Redis server endpoint available for SCAN/Keys.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Error Code 20007 - Redis server not available");
            return;
        }

        var contractKeys = server
            .Keys(pattern: ContractKeyPattern(companyId)) // uses SCAN under the hood
            .Select(k => (string)k)
            .ToList();

        if (contractKeys.Count == 0)
        {
            context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
            await context.Response.WriteAsync("Error Code 20004 - No contract for this company");
            return;
        }

        // 5) Iterate contracts: check active + (optional) not expired → then verify plan routes
        var nowUtc = DateTime.UtcNow;
        bool routeAllowed = false;
        long? allowedContractId = null;
        long? allowedPlanId = null;

        foreach (var cKey in contractKeys)
        {
            var cJson = await db.StringGetAsync(cKey);
            if (cJson.IsNullOrEmpty) continue;

            // NOTE: ensure Contract entity is JSON-serializable and matches stored shape
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var contract = JsonSerializer.Deserialize<ContractDto>(cJson!, options);
            if (contract is null) continue;

            // Business checks: active (+ optional expiration window)
            var isActive = contract.IsActive;
            var notExpired = contract.EndDate == null || contract.EndDate > nowUtc;
            if (!isActive || !notExpired) continue;

            // Retrieve plan for this contract
            var pJson = await db.StringGetAsync(PlanKey(contract.PlanId));
            if (pJson.IsNullOrEmpty) continue;

            // Fast check: does this plan include the current route?
            var prKey = PlanRouteKey(contract.PlanId, route.Id);
            if (await db.KeyExistsAsync(prKey))
            {
                routeAllowed = true;
                allowedContractId = contract.Id;
                allowedPlanId = contract.PlanId;
                break;
            }
        }

        if (!routeAllowed)
        {
            context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
            await context.Response.WriteAsync("Error Code 20005 - Route not covered by any active plan");
            return;
        }

        // 6) Success: log context and continue
        _logger.LogInformation(
            "Plan validated. UserId={UserId}, CompanyId={CompanyId}, RouteId={RouteId}, ContractId={ContractId}, PlanId={PlanId}, Path={Path}",
            userId, companyId, route.Id, allowedContractId, allowedPlanId, requestPath);

        await _next(context);
    }

    /// <summary>
    /// Selects a server endpoint for SCAN/Keys operations.
    /// Prefers a primary/writable endpoint; falls back to first available.
    /// </summary>
    private static IServer? GetReadableServer(IConnectionMultiplexer mux)
    {
        // Try to find a server marked as primary/writable
        foreach (var ep in mux.GetEndPoints(configuredOnly: true))
        {
            var server = mux.GetServer(ep);
            if (server.IsConnected && !server.IsSlave) // IsSlave = false means primary in older StackExchange.Redis
                return server;
        }

        // Fallback: first connected server
        foreach (var ep in mux.GetEndPoints(configuredOnly: true))
        {
            var server = mux.GetServer(ep);
            if (server.IsConnected) return server;
        }

        return null;
    }
}
