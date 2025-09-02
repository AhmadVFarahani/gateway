namespace Gateway.Application.Contract.Dtos;

public class UpdateContractRequest
{
    public long CompanyId{ get; set; }
    public long PlanId{ get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool AutoRenew { get; set; }
    public bool IsActive { get; set; }
}
