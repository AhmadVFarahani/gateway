namespace Gateway.Domain.Entities;

public class Contract : BaseEntity
{
    public long CompanyId { get; set; }
    public long PlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool AutoRenew { get; set; }
    public bool IsActive { get; set; }
    public string Description { get; set; } = string.Empty;

    public Company Company { get; set; } = null!;
    public Plan Plan { get; set; } = null!;
}
