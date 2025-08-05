using Gateway.Application.Routes.Dtos;
using Gateway.Domain.Enums;

namespace Gateway.Application.Plan.Dtos;

public class CreatePlanRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public decimal RequestPrice { get; set; }
    public PricingType PricingType { get; set; }
    public int MaxRequestsPerMonth { get; set; }
    public bool IsUnlimited { get; set; }
    public string Description { get; set; } = string.Empty;
}