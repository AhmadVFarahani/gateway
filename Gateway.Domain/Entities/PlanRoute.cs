namespace Gateway.Domain.Entities;

public class PlanRoute : BaseEntity
{
    public long PlanId { get; set; }
    public long RouteId { get; set; }
    public decimal? FlatPricePerCall { get; set; }
    /// <summary>
    /// [
    // { "max": 100, "price": 1 },
    // { "max": 500, "price": 0.75 },
    //{ "max": null, "price": 0.5 }  // > 500 calls
    //]
    /// </summary>
    public string? TieredPricingJson { get; set; }  // Optional JSON 
    public bool IsFree { get; set; }

    public Plan Plan { get; set; } = null!;
    public Route Route { get; set; } = null!;
}
