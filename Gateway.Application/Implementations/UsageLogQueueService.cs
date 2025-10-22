using Gateway.Application.Interfaces;
using Gateway.Application.Usage;
using Gateway.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace Gateway.Application.Implementations;

/// <summary>
/// Thread-safe, bounded in-memory queue for collecting UsageLogEvent objects.
/// </summary>
public class UsageLogQueueService : IUsageLogQueueService
{
    private readonly ILogger<UsageLogQueueService> _logger;
    private readonly Channel<UsageLog> _channel;
    private readonly int _capacity;

    public UsageLogQueueService(IOptions<UsageLogSettings> options, ILogger<UsageLogQueueService> logger)
    {
        _logger = logger;
        _capacity = options.Value.QueueCapacity;

        // Create a bounded channel to prevent memory overflow
        var channelOptions = new BoundedChannelOptions(_capacity)
        {
            SingleReader = true,
            SingleWriter = false,
            //FullMode = BoundedChannelFullMode.DropOldest // drop oldest to prevent blocking
            FullMode = BoundedChannelFullMode.Wait // backpressure
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
            _logger.LogWarning("Usage log queue is full — dropping oldest event (capacity {Capacity})", _capacity);
            return;
        }

        await Task.CompletedTask; // maintain async signature
    }

    /// <summary>
    /// Reads a batch of log events (up to maxItems) from the queue for background processing.
    /// </summary>
    public async Task<List<UsageLog>> DequeueBatchAsync(int maxItems, CancellationToken ct)
    {
        var items = new List<UsageLog>(maxItems);

        try
        {
            // Try to read the first item (waits until available or canceled)
            var firstItem = await _channel.Reader.ReadAsync(ct);
            items.Add(firstItem);

            // Drain additional items if available
            while (items.Count < maxItems && _channel.Reader.TryRead(out var item))
                items.Add(item);
        }
        catch (OperationCanceledException)
        {
            // graceful shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while reading from usage log queue.");
        }

        return items;
    }
}

