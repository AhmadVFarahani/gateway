namespace Gateway.Application.Usage;

public class UsageLogEvent
{
    public long CompanyId { get; set; }
    public long ContractId { get; set; }
    public long PlanId { get; set; }
    public long RouteId { get; set; }
    public int ResponseStatusCode { get; set; }
    public long DurationMs { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
