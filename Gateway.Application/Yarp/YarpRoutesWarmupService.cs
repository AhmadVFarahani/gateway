using Gateway.Application.Routes.Dtos;
using Gateway.Application.Services.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;


/// <summary>
/// Loads Route + Service data from Redis, joins them, and stores in memory for YARP.
/// </summary>

namespace Gateway.Application.Yarp;

/// <summary>
/// Loads all Route + Service data from Redis, builds route forwarding cache,
/// and reloads YARP dynamically once the data is ready.
/// </summary>
public class YarpRoutesWarmupService : BackgroundService
{
    private readonly ILogger<YarpRoutesWarmupService> _logger;
    private readonly IMemoryCache _memory;
    private readonly IConnectionMultiplexer _redis;
    private readonly DynamicYarpConfigProvider _yarpProvider;

    private const string RoutesCacheKey = "gateway:routes:forward";

    public YarpRoutesWarmupService(
        ILogger<YarpRoutesWarmupService> logger,
        IMemoryCache memory,
        IConnectionMultiplexer redis,
        DynamicYarpConfigProvider yarpProvider)
    {
        _logger = logger;
        _memory = memory;
        _redis = redis;
        _yarpProvider = yarpProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Wait 2 seconds for Redis to be ready
            await Task.Delay(2000, stoppingToken);

            _logger.LogInformation("🔄 [YARP Warmup] Starting to load routes and services from Redis...");

            var routes = await LoadRoutesAndServicesAsync(stoppingToken);
            if (!routes.Any())
            {
                _logger.LogWarning("⚠️ [YARP Warmup] No routes found in Redis. YARP will start with an empty config.");
                return;
            }

            // Cache result in memory
            _memory.Set(RoutesCacheKey, routes, TimeSpan.FromMinutes(30));

            // Trigger reload in YARP
            _yarpProvider.Reload();

            _logger.LogInformation("✅ [YARP Warmup] Successfully loaded {Count} routes into memory and reloaded YARP.", routes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [YARP Warmup] Failed to initialize routes from Redis.");
        }
    }

    private async Task<List<RouteForwardDto>> LoadRoutesAndServicesAsync(CancellationToken ct)
    {
        var db = _redis.GetDatabase();
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Fetch Redis data
        var routeEntries = await db.HashGetAllAsync("routes");
        var serviceEntries = await db.HashGetAllAsync("services");

        _logger.LogInformation("ℹ️ [YARP Warmup] Loaded {RoutesCount} routes and {ServicesCount} services from Redis.",
            routeEntries.Length, serviceEntries.Length);

        // Deserialize all records
        var routes = routeEntries
            .Where(x => !x.Value.IsNullOrEmpty)
            .Select(x => JsonSerializer.Deserialize<RouteDto>(x.Value!, jsonOptions))
            .Where(x => x != null)
            .ToList()!;

        var services = serviceEntries
            .Where(x => !x.Value.IsNullOrEmpty)
            .Select(x => JsonSerializer.Deserialize<ServiceDto>(x.Value!, jsonOptions))
            .Where(x => x != null)
            .ToList()!;

        if (!routes.Any() || !services.Any())
        {
            _logger.LogWarning("⚠️ [YARP Warmup] Missing data — routes or services not found.");
            return new List<RouteForwardDto>();
        }

        // Join Routes + Services to build forwarding config
        var joined = (from r in routes
                      join s in services on r.ServiceId equals s.Id
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
