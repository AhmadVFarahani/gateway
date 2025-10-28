using DocumentFormat.OpenXml.Wordprocessing;
using Gateway.Application.Cache;
using Gateway.Application.Interfaces.Cache;
using Gateway.Application.Routes.Dtos;
using Gateway.Application.Services.Dtos;
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
    private const string RoutesCacheKey = "gateway:routes:forward";

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
        // 🧹 Delete old cache key before writing new
        await db.KeyDeleteAsync(key);

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

        // 🧹 Clean up previous business cache keys
        await DeleteKeysByPatternAsync(db, new[] { "contract:*", "plan:*", "planroute:*" });

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


        // 🧹 Delete old hashes first
        await db.KeyDeleteAsync("routes");
        await db.KeyDeleteAsync("services");

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

        // Cache result in memory
        var routes = await JoinRoutesandServicesAsunc(data,ct);
        if (!routes.Any())
        {
            _logger.LogWarning("⚠️ [YARP Warmup] No routes found in Redis. YARP will start with an empty config.");
        }
        // Cache result in memory
        _memory.Set(RoutesCacheKey, routes, TimeSpan.FromMinutes(30));

        _logger.LogInformation("✅ YARP route/service cache stored in Redis: {RouteCount} routes, {ServiceCount} services",
            data.Routes.Count, data.Services.Count);
    }

    // 🔧 Helper: Delete all keys matching a pattern (safe SCAN-based delete)
    private async Task DeleteKeysByPatternAsync(IDatabase db, string[] patterns)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        foreach (var pattern in patterns)
        {
            var keys = server.Keys(pattern: pattern).ToArray();
            if (keys.Length > 0)
            {
                await db.KeyDeleteAsync(keys);
                _logger.LogInformation("🧹 Deleted {Count} keys with pattern '{Pattern}'", keys.Length, pattern);
            }
        }
    }

    private async Task<List<RouteForwardDto>> JoinRoutesandServicesAsunc(YarpRoteConfigCache data, CancellationToken ct)
    {
        var db = _redis.GetDatabase();
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Fetch Redis data
        var routeEntries = await db.HashGetAllAsync("routes");
        var serviceEntries = await db.HashGetAllAsync("services");

        _logger.LogInformation("ℹ️ [YARP Warmup] Loaded {RoutesCount} routes and {ServicesCount} services from Redis.",
            routeEntries.Length, serviceEntries.Length);



        if (!data.Routes.Any() || !data.Services.Any())
        {
            _logger.LogWarning("⚠️ [YARP Warmup] Missing data — routes or services not found.");
            return new List<RouteForwardDto>();
        }

        // Join Routes + Services to build forwarding config
        var joined = (from r in data.Routes
                      join s in data.Services on r.ServiceId equals s.Id
                      select new RouteForwardDto
                      {
                          Id = r.Id,
                          Path = NormalizePath(r.Path),
                          TargetPath = NormalizePath(r.TargetPath),
                          ServiceId = s.Id,
                          ServiceName = s.Name,
                          TargetBaseUrl = s.BaseUrl.TrimEnd('/'),
                          HttpMethod = r.HttpMethod
                      }).ToList();

        _logger.LogInformation("✅ [YARP Warmup] Joined {Count} route-service pairs.", joined.Count);

        return joined;
    }
    private static string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return "/";
        return path.Trim().TrimEnd('/').ToLowerInvariant();
    }
}

