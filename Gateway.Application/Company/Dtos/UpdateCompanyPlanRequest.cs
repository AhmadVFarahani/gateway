namespace Gateway.Application.Company.Dtos;

public class UpdateCompanyPlanRequest
{

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool AutoRenew { get; set; }
    public bool IsActive { get; set; }
}
