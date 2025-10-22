using Gateway.Application.Usage;
using Gateway.Domain.Entities;

namespace Gateway.Application.Interfaces;

public interface IUsageLogQueueService
{
    Task EnqueueAsync(UsageLog logEvent);
    Task<List<UsageLog>> DequeueBatchAsync(int maxItems, CancellationToken ct);
}
