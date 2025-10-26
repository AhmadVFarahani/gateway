using Gateway.Application.Interfaces;
using Gateway.Application.Usage;
using Gateway.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace Gateway.Application.Implementations;

/// <summary>
/// Thread-safe, bounded in-memory queue for collecting UsageLogEvent objects.
/// Includes Prometheus metrics for queue length and dropped events.
/// </summary>
public class UsageLogQueueService : IUsageLogQueueService
{
    private readonly ILogger<UsageLogQueueService> _logger;
    private readonly Channel<UsageLog> _channel;
    private readonly int _capacity;
    private readonly IMetricsService _metrics;

    private int _currentCount = 0; // manual counter for current queue length

    public UsageLogQueueService(
        IOptions<UsageLogSettings> options,
        ILogger<UsageLogQueueService> logger,
        IMetricsService metrics)
    {
        _logger = logger;
        _metrics = metrics;
        _capacity = options.Value.QueueCapacity;

        // Create a bounded channel to prevent memory overflow
        var channelOptions = new BoundedChannelOptions(_capacity)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait // Apply backpressure
        };

        _channel = Channel.CreateBounded<UsageLog>(channelOptions);
        _logger.LogInformation("Usage log queue initialized with capacity {Capacity}", _capacity);
    }

    /// <summary>
    /// Adds a log event to the in-memory queue (non-blocking).
    /// </summary>
    public async Task EnqueueAsync(UsageLog logEvent)
    {
        if (!_channel.Writer.TryWrite(logEvent))
        {
            // Queue full - increment drop counter
            _metrics.IncQueueDrop();
            _logger.LogWarning("Usage log queue is full — dropping event (capacity {Capacity})", _capacity);
            return;
        }

        Interlocked.Increment(ref _currentCount);
        _metrics.SetQueueLength(_currentCount);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Reads a batch of log events (up to maxItems) from the queue for background processing.
    /// </summary>
    public async Task<List<UsageLog>> DequeueBatchAsync(int maxItems, CancellationToken ct)
    {
        var items = new List<UsageLog>(maxItems);

        try
        {
            // Wait for the first available item
            var firstItem = await _channel.Reader.ReadAsync(ct);
            items.Add(firstItem);
            Interlocked.Decrement(ref _currentCount);

            // Drain additional items if available
            while (items.Count < maxItems && _channel.Reader.TryRead(out var item))
            {
                items.Add(item);
                Interlocked.Decrement(ref _currentCount);
            }

            // Update gauge after batch read
            _metrics.SetQueueLength(_currentCount);
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while reading from usage log queue.");
        }

        return items;
    }
}