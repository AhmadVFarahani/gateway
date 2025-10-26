using Gateway.Application.Interfaces;
using Gateway.Application.Usage;
using Gateway.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Gateway.Application.BackgroundServices;

/// <summary>
/// Background worker that continuously reads usage logs from the in-memory queue
/// and batch-inserts them into SQL using a repository.
/// Includes Prometheus histogram for bulk insert durations.
/// </summary>
public class UsageLogBackgroundService : BackgroundService
{
    private readonly ILogger<UsageLogBackgroundService> _logger;
    private readonly IUsageLogQueueService _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly UsageLogSettings _settings;
    private readonly IMetricsService _metrics;

    public UsageLogBackgroundService(
        ILogger<UsageLogBackgroundService> logger,
        IUsageLogQueueService queue,
        IServiceScopeFactory scopeFactory,
        IOptions<UsageLogSettings> settings,
        IMetricsService metrics)
    {
        _logger = logger;
        _queue = queue;
        _scopeFactory = scopeFactory;
        _settings = settings.Value;
        _metrics = metrics;
    }

    /// <summary>
    /// Main background loop: periodically dequeues batches of usage logs
    /// and inserts them into SQL in bulk. Cancels gracefully on shutdown.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "✅ UsageLogBackgroundService started. BatchSize={BatchSize}, Interval={Interval}s",
            _settings.BatchSize,
            _settings.FlushIntervalSeconds);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(_settings.FlushIntervalSeconds), stoppingToken);

                var batch = await _queue.DequeueBatchAsync(_settings.BatchSize, stoppingToken);
                if (batch.Count == 0)
                    continue;

                // Measure performance using Prometheus histogram
                var stopwatch = Stopwatch.StartNew();

                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IUsageLogRepository>();

                await repo.BulkInsertAsync(batch, stoppingToken);

                stopwatch.Stop();
                _metrics.BulkInsertHistogram.Observe(stopwatch.Elapsed.TotalSeconds);

                _logger.LogInformation("💾 Inserted {Count} usage logs in {Elapsed} ms",
                    batch.Count, stopwatch.ElapsedMilliseconds);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("⚙️ Application stopping — initiating graceful drain...");
            await DrainRemainingLogsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Unexpected error in UsageLogBackgroundService.");
        }

        _logger.LogInformation("🛑 UsageLogBackgroundService stopped gracefully.");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🛑 StopAsync triggered — flushing remaining usage logs...");
        await DrainRemainingLogsAsync();
        await base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Drains remaining log events from the queue before shutdown to avoid data loss.
    /// </summary>
    private async Task DrainRemainingLogsAsync()
    {
        try
        {
            int totalDrained = 0;
            var sw = Stopwatch.StartNew();

            while (true)
            {
                var batch = await _queue.DequeueBatchAsync(_settings.BatchSize, CancellationToken.None);
                if (batch.Count == 0)
                    break;

                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IUsageLogRepository>();
                await repo.BulkInsertAsync(batch, CancellationToken.None);

                _metrics.BulkInsertHistogram.Observe(sw.Elapsed.TotalSeconds);
                totalDrained += batch.Count;
                await Task.Delay(100);
            }

            sw.Stop();
            _logger.LogInformation("✅ Graceful drain completed — {Count} logs persisted in {Elapsed} ms",
                totalDrained, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Error during graceful drain. Some logs may not have been saved.");
        }
    }
}