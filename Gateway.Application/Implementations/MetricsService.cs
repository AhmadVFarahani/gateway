using Gateway.Application.Interfaces;
using Prometheus;

namespace Gateway.Application.Implementations;

public class MetricsService : IMetricsService
{
    public Counter GatewayRequestCounter { get; }
    public MetricsService()
    {
        GatewayRequestCounter = Metrics.CreateCounter(
            "gateway_requests_total",
            "Number of HTTP requests processed by Gateway (with labels)",
            new CounterConfiguration
            {
                LabelNames = new[] { "user_id", "apikey_id", "path", "method", "status_code" }
            });
    }
    // Gauge: current number of items in queue
    private readonly Gauge _queueLengthGauge = Metrics.CreateGauge(
        "gateway_usage_queue_length",
        "Current length of the usage log queue");

    // Counter: total number of dropped events (if any)
    private readonly Counter _queueDrops = Metrics.CreateCounter(
        "gateway_usage_queue_drops_total",
        "Total number of dropped events from the usage queue");

    // Histogram: durations of bulk insert operations (seconds)
    public IHistogram BulkInsertHistogram { get; } = Metrics.CreateHistogram(
        "gateway_bulk_insert_duration_seconds",
        "Bulk insert duration in seconds",
        new HistogramConfiguration
        {
            // buckets are in seconds; tune as needed
            Buckets = Histogram.ExponentialBuckets(start: 0.01, factor: 2, count: 10)
        });

    public void SetQueueLength(int length) => _queueLengthGauge.Set(length);
    public void IncQueueDrop() => _queueDrops.Inc();
}