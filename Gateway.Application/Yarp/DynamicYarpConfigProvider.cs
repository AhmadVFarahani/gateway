using Gateway.Application.Routes.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Gateway.Application.Yarp;

/// <summary>
/// Dynamically builds YARP route and cluster configurations from cached RouteForwardDto objects.
/// </summary>
public class DynamicYarpConfigProvider : IProxyConfigProvider
{
    private readonly IMemoryCache _memory;
    private readonly ILogger<DynamicYarpConfigProvider> _logger;

    // Holds the current in-memory proxy configuration
    private volatile InMemoryConfig _currentConfig;

    // Used to notify YARP when configuration changes
    private CancellationTokenSource _cts = new();

    // Cache key where route data (List<RouteForwardDto>) is stored
    private const string RoutesCacheKey = "gateway:routes:forward";

    public DynamicYarpConfigProvider(IMemoryCache memory, ILogger<DynamicYarpConfigProvider> logger)
    {
        _memory = memory;
        _logger = logger;

        // Build initial configuration from memory cache
        _currentConfig = BuildConfig();
    }

    /// <summary>
    /// Returns the current in-memory YARP configuration.
    /// </summary>
    public IProxyConfig GetConfig() => _currentConfig;

    /// <summary>
    /// Forces a full reload of YARP routes and clusters (called after Redis or DB cache refresh).
    /// </summary>
    public void Reload()
    {
        _logger.LogInformation("🔄 Reloading YARP dynamic configuration...");
        var oldCts = _cts;

        _cts = new CancellationTokenSource();
        _currentConfig = BuildConfig();

        oldCts.Cancel();
        oldCts.Dispose();
    }

    /// <summary>
    /// Reads route data from IMemoryCache and constructs YARP routes and clusters.
    /// </summary>
    private InMemoryConfig BuildConfig()
    {
        var routes = _memory.Get<List<RouteForwardDto>>(RoutesCacheKey) ?? new();
        var changeToken = new CancellationChangeToken(_cts.Token);

        if (!routes.Any())
        {
            _logger.LogWarning("⚠️ No routes found in cache for YARP configuration.");
            return new InMemoryConfig(Array.Empty<RouteConfig>(), Array.Empty<ClusterConfig>(), changeToken);
        }

        // Build YARP Route configurations
        var routeConfigs = routes.Select(r => new RouteConfig
        {
            RouteId = $"route-{r.Id}",
            ClusterId = $"cluster-{r.ServiceName}",
            Match = new RouteMatch
            {
                // Public path pattern (client → gateway)
                Path = r.Path, // ✅ use external Path, not TargetPath
                Methods = new[] { r.HttpMethod.ToString() }
            },
            // Transform rewrites path to internal target before forwarding
            Transforms = new[]
            {
                new Dictionary<string, string> { ["PathPattern"] = r.TargetPath }
            }
        }).ToList();

        // Build YARP Cluster configurations (grouped by service)
        var clusterConfigs = routes
            .GroupBy(r => r.ServiceName)
            .Select(g => new ClusterConfig
            {
                ClusterId = $"cluster-{g.Key}",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    // Base URL of the downstream service
                    { $"dest-{g.Key}", new DestinationConfig { Address = g.First().TargetBaseUrl.TrimEnd('/') } }
                }
            })
            .ToList();

        _logger.LogInformation("✅ YARP configuration built: {Routes} routes, {Clusters} clusters",
            routeConfigs.Count, clusterConfigs.Count);

        return new InMemoryConfig(routeConfigs, clusterConfigs, changeToken);
    }

    /// <summary>
    /// Internal in-memory proxy config used by YARP.
    /// </summary>
    private sealed class InMemoryConfig : IProxyConfig
    {
        public InMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters, IChangeToken changeToken)
        {
            Routes = routes;
            Clusters = clusters;
            ChangeToken = changeToken;
        }

        public IReadOnlyList<RouteConfig> Routes { get; }
        public IReadOnlyList<ClusterConfig> Clusters { get; }
        public IChangeToken ChangeToken { get; }
    }
}
