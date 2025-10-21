using Gateway.Application.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Gateway.Public.Api.Controllers;

[ApiController]
[Route("api/cachetest")]
public class CacheTestController : ControllerBase
{
    private readonly IMemoryCache _memory;

    public CacheTestController(IMemoryCache memory) => _memory = memory;

    [HttpGet("auth")]
    public IActionResult GetAuthCache()
    {
        const string key = "gateway:auth:config";

        if (_memory.TryGetValue(key, out GatewayConfigCache? cache))
        {
            return Ok(new
            {
                status = "OK",
                users = cache.Users.Count,
                routes = cache.Routes.Count,
                scopes = cache.Scopes.Count,
                routeScopes = cache.RouteScopes.Count,
                accessPolicies = cache.AccessPolicies.Count,
                apiKeys= cache.ApiKeys.Count,
            });
        }

        return NotFound("Auth cache not found in memory.");
    }

    [HttpGet("plan")]
    public IActionResult GetPlanCache()
    {
        const string contractKey = "contract:some-id"; // Optional
                                                       // یا فقط وجود MemoryCache را بررسی کنیم
        var memoryKeys = _memory.GetType().GetField("_entries",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(_memory) as System.Collections.IDictionary;

        return Ok(new
        {
            memoryKeysCount = memoryKeys?.Count ?? 0
        });
    }
}
