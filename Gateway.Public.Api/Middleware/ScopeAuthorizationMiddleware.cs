using Gateway.Domain.Interfaces;

namespace Gateway.Public.Api.Middleware;

public class ScopeAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public ScopeAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context,
        IRouteRepository routeRepository
         )
    { 
        
        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
          
        }

        var userId = context.User.FindFirst("UserId")?.Value;
        var apiKeyId = context.User.FindFirst("ApiKeyId")?.Value;

        var path = context.Request.Path.Value?.ToLower() ?? "";

        var route = await routeRepository.GetByPathAsync(path);
        if (route == null)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Route not found");
            return;
        }

        //var requiredScopeIds = (await routeScopeRepository.GetByRouteIdAsync(route.Id))
        //    .Select(x => x.ScopeId)
        //    .ToList();

        //if (!requiredScopeIds.Any())
        //{
        //    await _next(context);
        //    return;
        //}

        //var accessPolicies = await accessPolicyRepository.GetAllAsync();
        //var userScopes = accessPolicies
        //    .Where(x =>
        //        x.IsActive &&
        //        (x.UserId.ToString() == userId || x.ApiKeyId.ToString() == apiKeyId))
        //    .Select(x => x.ScopeId)
        //    .ToHashSet();

        //var isAuthorized = requiredScopeIds.Any(scope => userScopes.Contains(scope));

        //if (!isAuthorized)
        //{
        //    context.Response.StatusCode = 403;
        //    await context.Response.WriteAsync("Forbidden: insufficient scope.");
        //    return;
        //}

        await _next(context);
    }
}
