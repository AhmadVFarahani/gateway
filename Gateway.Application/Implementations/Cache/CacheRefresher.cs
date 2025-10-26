using DocumentFormat.OpenXml.Wordprocessing;
using Gateway.Application.Interfaces.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Gateway.Application.Implementations.Cache;

public class CacheRefresher : ICacheRefresher
{
    private readonly ICacheLoader _loader;
    private readonly IMemoryCache _memory;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<CacheRefresher> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public CacheRefresher(
        ICacheLoader loader,
        IMemoryCache memory,
        IConnectionMultiplexer redis,
        ILogger<CacheRefresher> logger)
    {
        _loader = loader;
        _memory = memory;
        _redis = redis;
        _logger = logger;
    }

    public async Task RefreshAuthorizationCacheAsync(CancellationToken ct = default)
    {
        var db = _redis.GetDatabase();
        var key = "gateway:auth:config";
        _logger.LogInformation("Refreshing Authorization cache...");

        var data = await _loader.LoadAuthorizationDataAsync(ct);
        var json = JsonSerializer.Serialize(data, _jsonOptions);

        await db.StringSetAsync(key, json);
        _memory.Set(key, data, new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove
        });

        _logger.LogInformation("Authorization cache refreshed and stored in Redis + Memory.");
    }

    public async Task RefreshBusinessCacheAsync(CancellationToken ct = default)
    {
        var db = _redis.GetDatabase();
        _logger.LogInformation("Refreshing Business cache (Plans/Contracts)...");

        var data = await _loader.LoadBusinessDataAsync(ct);

        // Store in Redis as separate hashes
        var batch = db.CreateBatch();

        foreach (var contract in data.Contracts)
            batch.StringSetAsync($"contract:{contract.CompanyId}:{contract.Id}", JsonSerializer.Serialize(contract, _jsonOptions));

        foreach (var plan in data.Plans)
            batch.StringSetAsync($"plan:{plan.Id}", JsonSerializer.Serialize(plan, _jsonOptions));

        foreach (var pr in data.PlanRoutes)
            batch.StringSetAsync($"planroute:{pr.PlanId}:{pr.RouteId}", JsonSerializer.Serialize(pr, _jsonOptions));

        batch.Execute();
        _logger.LogInformation("Business cache refreshed in Redis.");
    }

    // 🔹 YARP ROUTE/SERVICE CACHE
    public async Task RefreshYarpCacheAsync(CancellationToken ct = default)
    {
        var db = _redis.GetDatabase();
        _logger.LogInformation("Refreshing YARP route/service cache...");

        var data = await _loader.LoadYarpDataAsync(ct);

        if (!data.Routes.Any() || !data.Services.Any())
        {
            _logger.LogWarning("⚠️ No routes or services found in database.");
            return;
        }

        // Routes → Hash (routes)
        foreach (var r in data.Routes)
        {
            await db.HashSetAsync("routes", r.Id.ToString(),
                JsonSerializer.Serialize(r, _jsonOptions));
        }

        // Services → Hash (services)
        foreach (var s in data.Services)
        {
            await db.HashSetAsync("services", s.Id.ToString(),
                JsonSerializer.Serialize(s, _jsonOptions));
        }

        _logger.LogInformation("✅ YARP route/service cache stored in Redis: {RouteCount} routes, {ServiceCount} services",
            data.Routes.Count, data.Services.Count);
    }
}