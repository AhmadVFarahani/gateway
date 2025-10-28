using Gateway.Application.Routes.Dtos;
using Gateway.Application.Services.Dtos;
using Gateway.Application.Yarp;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;


/// <summary>
/// Loads Route + Service data from Redis, joins them, and stores in memory for YARP.
/// </summary>

namespace Gateway.Application.BackgroundServices;

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

            

           
            // Trigger reload in YARP
            _yarpProvider.Reload();

            _logger.LogInformation("✅ [YARP Warmup] Successfully loaded  routes into memory and reloaded YARP.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [YARP Warmup] Failed to initialize routes from Redis.");
        }
    }

   
    
}
