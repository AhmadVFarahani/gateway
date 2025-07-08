using Gateway.Domain.Enums;

namespace Gateway.Domain.Entities;

public class Company:BaseEntity
{
    
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<CompanyPlan> CompanyPlans { get; set; } = new List<CompanyPlan>();
    public ICollection<CompanyRoutePricing> CompanyRoutePricings { get; set; } = new List<CompanyRoutePricing>();




    public void addPlan(CompanyPlan plan)
    {
        CompanyPlans.Add(plan);
    }

    public void addRoutePricing(CompanyRoutePricing routePricing)
    {
        CompanyRoutePricings.Add(routePricing);
    }

    public void updatePlan(long id, DateTime startDate, DateTime? endDate, bool autoRenew, bool isActive)
    {
        var plan = CompanyPlans.FirstOrDefault(p => p.Id == id) 
            ?? throw new KeyNotFoundException("Company plan not found");

        plan.EndDate = endDate;
        plan.StartDate = startDate;
        plan.IsActive = isActive;
        plan.AutoRenew = autoRenew;
        plan.UpdatedAt  = DateTime.UtcNow;
    }

    public void updateRoutePricing(long id, BillingType billingType, long routeId, decimal? pricePerCall,
        int? MaxFreeCallsPerMonth,
         int? MaxFreeCalls,
         string? TieredJson,
         decimal? monthlySubscriptionPrice,
        bool isActive)
    {
        var routePricing = CompanyRoutePricings.FirstOrDefault(p => p.Id == id)
            ?? throw new KeyNotFoundException("Company Route Pricing not found");

        routePricing.BillingType = billingType; 
        routePricing.RouteId = routeId;
        routePricing.PricePerCall = pricePerCall;
        routePricing.MaxFreeCallsPerMonth = MaxFreeCallsPerMonth;
        routePricing.MaxFreeCalls = MaxFreeCalls;
        routePricing.TieredJson = TieredJson;
        routePricing.MonthlySubscriptionPrice = monthlySubscriptionPrice;
        routePricing.IsActive = isActive;
        routePricing.UpdatedAt = DateTime.UtcNow;

    }
    public void deleteRoutePricing(long id)
    {
        var routePricing = CompanyRoutePricings.FirstOrDefault(s => s.Id == id)
            ?? throw new KeyNotFoundException("route not found");
        CompanyRoutePricings.Remove(routePricing);
    }
}
