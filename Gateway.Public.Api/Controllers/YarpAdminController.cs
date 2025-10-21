using Gateway.Application.Routes.Dtos;
using Gateway.Application.Yarp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace Gateway.Public.Api.Controllers;

[ApiController]
[Route("api/yarp")]
public class YarpAdminController : ControllerBase
{
    private readonly IMemoryCache _memory;
    private readonly DynamicYarpConfigProvider _provider;
    private readonly IConnectionMultiplexer _redis;

    private const string RoutesCacheKey = "gateway:routes:forward";

    public YarpAdminController(IMemoryCache memory, DynamicYarpConfigProvider provider, IConnectionMultiplexer redis)
    {
        _memory = memory;
        _provider = provider;
        _redis = redis;
    }

    [HttpPost("reload")]
    public async Task<IActionResult> Reload()
    {
        // Reload routes from Redis -> Memory
        var db = _redis.GetDatabase();
        var entries = await db.HashGetAllAsync("routes:forward");

        var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var routes = entries
            .Where(e => !e.Value.IsNullOrEmpty)
            .Select(e => System.Text.Json.JsonSerializer.Deserialize<RouteForwardDto>(e.Value!, options))
            .Where(x => x != null)
            .ToList()!;

        _memory.Set(RoutesCacheKey, routes, TimeSpan.FromMinutes(10));

        // Signal YARP
        _provider.Reload();

        return Ok(new { reloaded = true, count = routes.Count });
    }
}