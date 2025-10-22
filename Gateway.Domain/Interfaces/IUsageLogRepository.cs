using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IUsageLogRepository
{
    Task BulkInsertAsync(IEnumerable<UsageLog> logs, CancellationToken ct);
}