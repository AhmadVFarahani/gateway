using Gateway.Domain.Enums;

namespace Gateway.Domain.Entities;

public class CompanyRoutePricing : BaseEntity
{
    public long CompanyId { get; set; }
    public long RouteId { get; set; }
    public decimal? PricePerCall { get; set; }
    public int? MaxFreeCallsPerMonth { get; set; }
    public BillingType BillingType { get; set; } = BillingType.Fixed;
    public int? MaxFreeCalls { get; set; }
    public string? TieredJson { get; set; }  // If BillingType == "Tiered"
    public decimal? MonthlySubscriptionPrice { get; set; }

    public bool IsActive { get; set; }

    public Company Company { get; set; } = null!;
    public Route Route { get; set; } = null!;
}