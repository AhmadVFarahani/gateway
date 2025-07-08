using Gateway.Domain.Enums;

namespace Gateway.Application.Company.Dtos;

public class CreateCompanyRoutePricingRequest
{
    public long RouteId { get; set; }
    public decimal? PricePerCall { get; set; }
    public int? MaxFreeCallsPerMonth { get; set; }
    public BillingType BillingType { get; set; } = BillingType.Fixed;
    public int? MaxFreeCalls { get; set; }
    public string? TieredJson { get; set; }  // If BillingType == "Tiered"
    public decimal? MonthlySubscriptionPrice { get; set; }

    public bool IsActive { get; set; }
}
