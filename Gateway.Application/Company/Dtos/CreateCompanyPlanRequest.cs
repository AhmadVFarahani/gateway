namespace Gateway.Application.Company.Dtos;

public class CreateCompanyPlanRequest
{
    public long PlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool AutoRenew { get; set; }
    public bool IsActive { get; set; }
}
