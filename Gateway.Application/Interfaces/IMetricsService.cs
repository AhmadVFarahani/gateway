using Prometheus;

namespace Gateway.Application.Interfaces;

public interface IMetricsService
{
    Counter GatewayRequestCounter { get; }
    void SetQueueLength(int length);
    void IncQueueDrop();
    IHistogram? BulkInsertHistogram { get; } // for measuring bulk durations
}
