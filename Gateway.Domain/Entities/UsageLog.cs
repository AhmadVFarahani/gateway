namespace Gateway.Domain.Entities;

public class UsageLog: BaseEntity
{
    public long KeyId { get; set; }
    public long UserId { get; set; }
    public long CompanyId { get; set; }
    public long ContractId { get; set; } // 
    public long PlanId { get; set; }     //
    public long RouteId { get; set; }    //

    //public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public int ResponseStatusCode { get; set; }

    public long DurationMs { get; set; } //

    public bool IsBilled { get; set; } = false; // 
}