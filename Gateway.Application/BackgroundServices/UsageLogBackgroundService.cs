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
/// Supports graceful shutdown: remaining logs are flushed before the app stops.
/// </summary>
public class UsageLogBackgroundService : BackgroundService
{
    private readonly ILogger<UsageLogBackgroundService> _logger;
    private readonly IUsageLogQueueService _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly UsageLogSettings _settings;

    public UsageLogBackgroundService(
        ILogger<UsageLogBackgroundService> logger,
        IUsageLogQueueService queue,
        IServiceScopeFactory scopeFactory,
        IOptions<UsageLogSettings> settings)
    {
        _logger = logger;
        _queue = queue;
        _scopeFactory = scopeFactory;
        _settings = settings.Value;
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
            _settings.FlushIntervalSeconds
        );

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Wait for the configured flush interval
                await Task.Delay(TimeSpan.FromSeconds(_settings.FlushIntervalSeconds), stoppingToken);

                // Try to dequeue a batch of logs
                var batch = await _queue.DequeueBatchAsync(_settings.BatchSize, stoppingToken);
                if (batch.Count == 0)
                    continue;

                // Measure performance for diagnostic logs
                var stopwatch = Stopwatch.StartNew();

                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IUsageLogRepository>();

                await repo.BulkInsertAsync(batch, stoppingToken);

                stopwatch.Stop();
                _logger.LogInformation("💾 Inserted {Count} usage logs in {Elapsed} ms",
                    batch.Count, stopwatch.ElapsedMilliseconds);
            }
        }
        catch (OperationCanceledException)
        {
            // Triggered when the application is stopping
            _logger.LogInformation("⚙️ Application stopping — initiating graceful drain...");
            await DrainRemainingLogsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Unexpected error in UsageLogBackgroundService.");
        }

        _logger.LogInformation("🛑 UsageLogBackgroundService stopped gracefully.");
    }

    /// <summary>
    /// Called automatically when the host is stopping.
    /// Ensures all remaining logs are flushed even if ExecuteAsync was still in a delay cycle.
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🛑 StopAsync triggered — flushing remaining usage logs...");
        await DrainRemainingLogsAsync();
        await base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Drains remaining log events from the in-memory queue before shutdown
    /// to avoid data loss. Runs synchronously until the queue is empty.
    /// </summary>
    private async Task DrainRemainingLogsAsync()
    {
        try
        {
            int totalDrained = 0;
            var sw = Stopwatch.StartNew();

            while (true)
            {
                // Fetch next batch from queue (no cancellation here)
                var batch = await _queue.DequeueBatchAsync(_settings.BatchSize, CancellationToken.None);
                if (batch.Count == 0)
                    break;

                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IUsageLogRepository>();
                await repo.BulkInsertAsync(batch, CancellationToken.None);

                totalDrained += batch.Count;

                // Optional short delay to avoid DB overload on massive queues
                await Task.Delay(100);
            }

            sw.Stop();
            _logger.LogInformation(
                "✅ Graceful drain completed — {Count} logs persisted in {Elapsed} ms",
                totalDrained,
                sw.ElapsedMilliseconds
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Error during graceful drain. Some logs may not have been saved.");
        }
    }
}