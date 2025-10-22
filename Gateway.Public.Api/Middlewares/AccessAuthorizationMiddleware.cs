using Gateway.Application.Cache;
using Gateway.Application.Routes.Dtos;
using Gateway.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace Gateway.Public.Api.Middlewares;

public class AccessAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AccessAuthorizationMiddleware> _logger;
    private readonly IMemoryCache _memory;

    public AccessAuthorizationMiddleware(
        RequestDelegate next,
        ILogger<AccessAuthorizationMiddleware> logger,
        IMemoryCache memory)
    {
        _next = next;
        _logger = logger;
        _memory = memory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1️⃣ Ensure JWT authentication
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await RejectAsync(context, 401, "Unauthorized");
            return;
        }

        // 2️⃣ Load cache
        if (!_memory.TryGetValue("gateway:auth:config", out GatewayConfigCache? config) || config == null)
        {
            _logger.LogError("Authorization cache not found or empty.");
            await RejectAsync(context, 500, "Error Code 10000 - Config cache missing");
            return;
        }

        // 3️⃣ Extract claims from JWT
        var userId = GetLongClaim(context, "UserId");
        var apiKeyId = GetLongClaim(context, "ApiKeyId");

       


        if (userId == 0)
        {
            await RejectAsync(context, 401, "Error Code 10001 - Missing UserId");
            return;
        }

        if (apiKeyId == 0)
        {
            await RejectAsync(context, 401, "Error Code 10002 - Missing ApiKeyId");
            return;
        }

       // 4️⃣ Validate ApiKey ownership
        var apiKey = config.ApiKeys.FirstOrDefault(k => k.Id == apiKeyId && k.UserId == userId && k.IsActive);
        if (apiKey == null)
        {
            await RejectAsync(context, 403, "Error Code 10003 - Invalid or inactive ApiKey");
            return;
        }

        // 5️⃣ Normalize method and path
        var requestPath = context.Request.Path.Value?.TrimEnd('/').ToLowerInvariant() ?? "";
        var requestMethod = context.Request.Method.ToUpperInvariant();

        if (!Enum.TryParse<HttpMethodEnum>(requestMethod, true, out var methodEnum))
        {
            await RejectAsync(context, 405, "Error Code 10006 - Unsupported HTTP method");
            return;
        }

        // 6️⃣ Find matching route using dynamic path matching
        var route = MatchRoute(config.Routes, requestPath, methodEnum);
        if (route == null)
        {
            await RejectAsync(context, 404, "Error Code 10004 - Route not found");
            return;
        }

        // Save route in HttpContext for next middleware (e.g., PlanValidation)
        context.Items["RouteInfo"] = route;

        // 7️⃣ Get all active policies for this user + ApiKey
        var activePolicies = config.AccessPolicies
            .Where(p => p.UserId == userId && p.ApiKeyId == apiKey.Id && p.IsActive)
            .ToList();

        if (!activePolicies.Any())
        {
            await RejectAsync(context, 403, "Error Code 10005 - No active access policy");
            return;
        }

        var allowedScopeIds = activePolicies.Select(p => p.ScopeId).Distinct().ToList();
        var routeScopeIds = config.RouteScopes.Where(rs => rs.RouteId == route.Id).Select(rs => rs.ScopeId);

        // 8️⃣ Scope validation
        if (!routeScopeIds.Any(rs => allowedScopeIds.Contains(rs)))
        {
            await RejectAsync(context, 403, "Error Code 10007 - Scope mismatch");
            return;
        }


        //add to context for later middlewares
        context.Items["UserId"] = userId;
        context.Items["ApiKeyId"] = apiKeyId;
        context.Items["RouteId"] = route.Id;
        context.Items["UserId"] = userId;
        context.Items["KeyId"] = apiKeyId;


        // ✅ Access granted
        _logger.LogInformation(
            "Access granted. UserId={UserId}, ApiKeyId={ApiKeyId}, Route={RoutePath}, Method={Method}",
            userId, apiKeyId, route.Path, requestMethod);

        await _next(context);
    }

    // 🔹 Helper: Extract numeric claims safely
    private static long GetLongClaim(HttpContext context, string claimType)
    {
        return long.TryParse(context.User.FindFirst(claimType)?.Value, out var value) ? value : 0;
    }

    // 🔹 Helper: Return error responses consistently
    private static async Task RejectAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(message);
    }

    // 🔹 Helper: Match routes including dynamic segments (e.g., /api/Company/{id})
    private static RouteDto MatchRoute(
        IEnumerable<RouteDto> routes,
        string requestPath,
        HttpMethodEnum method)
    {
        var requestSegments = requestPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        foreach (var r in routes)
        {
            if (r.HttpMethod != method)
                continue;

            var patternSegments = r.Path
                .TrimEnd('/')
                .ToLowerInvariant()
                .Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (patternSegments.Length != requestSegments.Length)
                continue;

            bool isMatch = true;
            for (int i = 0; i < patternSegments.Length; i++)
            {
                var pattern = patternSegments[i];
                var request = requestSegments[i];

                // If pattern segment is {something}, accept any value
                if (pattern.StartsWith("{") && pattern.EndsWith("}"))
                    continue;

                if (!string.Equals(pattern, request, StringComparison.OrdinalIgnoreCase))
                {
                    isMatch = false;
                    break;
                }
            }

            if (isMatch)
                return r;
        }

        return null;
    }
}