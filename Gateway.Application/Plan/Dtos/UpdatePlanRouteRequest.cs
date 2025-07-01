namespace Gateway.Application.Plan.Dtos;

public class UpdatePlanRouteRequest
{
    public decimal? FlatPricePerCall { get; set; }
    public string? TieredPricingJson { get; set; }
    public bool IsFree { get; set; }
    public long PlanId { get; set; }
}
