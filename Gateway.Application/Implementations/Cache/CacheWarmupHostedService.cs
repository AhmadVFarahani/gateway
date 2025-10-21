using Gateway.Application.Interfaces.Cache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gateway.Application.Implementations.Cache;

public class CacheWarmupHostedService : BackgroundService
{
    private readonly ILogger<CacheWarmupHostedService> _logger;
     private readonly IServiceProvider _sp;

    public CacheWarmupHostedService(ILogger<CacheWarmupHostedService> logger, IServiceProvider sp)
    {
        _logger = logger;
        _sp = sp;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Cache warm-up...");

        using var scope = _sp.CreateScope();
        var refresher = scope.ServiceProvider.GetRequiredService<ICacheRefresher>();

        await refresher.RefreshAuthorizationCacheAsync(stoppingToken);
        await refresher.RefreshBusinessCacheAsync(stoppingToken);

        _logger.LogInformation("Cache warm-up completed successfully.");
    }
}