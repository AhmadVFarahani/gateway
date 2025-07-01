using Gateway.Application.Base;

namespace Gateway.Application.Company.Dtos;

public class CompanyPlanDto:BaseDto
{
    public long PlanId { get; set; }
    public long CompanyId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool AutoRenew { get; set; }
    public bool IsActive { get; set; }
}
