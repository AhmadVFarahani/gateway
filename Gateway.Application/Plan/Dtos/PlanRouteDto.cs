using Gateway.Application.Base;
using Gateway.Application.Routes.Dtos;
namespace Gateway.Application.Plan.Dtos;

public class PlanRouteDto : BaseDto
{
    public long PlanId { get; set; } 
    public long RouteId { get; set; }
    public decimal? FlatPricePerCall { get; set; }
    public string? TieredPricingJson { get; set; }
    public bool IsFree { get; set; }
    //public PlanDto Plan { get; set; } = null!;
    //public RouteDto Route { get; set; } = null!;
}