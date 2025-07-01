using Gateway.Application.Base;
namespace Gateway.Application.Plan.Dtos;

public class PlanDto:BaseDto
{
    public string Name { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public int MaxRequestsPerMonth { get; set; }
    public bool IsUnlimited { get; set; }
    public string Description{ get; set; } = string.Empty;
}
