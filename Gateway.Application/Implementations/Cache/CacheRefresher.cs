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
        _memory.Set(key, data, TimeSpan.FromMinutes(10));

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
}