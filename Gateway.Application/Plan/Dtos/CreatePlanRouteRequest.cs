namespace Gateway.Application.Plan.Dtos;

public class CreatePlanRouteRequest
{
    public long PlanId { get; set; }
    public long RouteId { get; set; }
    public decimal? FlatPricePerCall { get; set; }
    public string? TieredPricingJson { get; set; }
    public bool IsFree { get; set; }
}
