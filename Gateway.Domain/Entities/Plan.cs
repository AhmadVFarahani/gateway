using System.ComponentModel;
using System.Numerics;

namespace Gateway.Domain.Entities;

public class Plan : BaseEntity
{
    public string Name { get; set; } = null!;
    public decimal MonthlyPrice { get; set; }
    public int MaxRequestsPerMonth { get; set; }
    public bool IsUnlimited { get; set; }
    public string Description { get; set; } = string.Empty;

    public ICollection<PlanRoute> PlanRoutes { get; set; } = new List<PlanRoute>();
    public ICollection<CompanyPlan> CompanyPlans { get; set; } = new List<CompanyPlan>();



    public void addRoute(PlanRoute planRoute)
    {
        if (PlanRoutes.Any(pr => pr.RouteId == planRoute.RouteId))
            throw new InvalidOperationException("This route is already associated with the plan.");

        PlanRoutes.Add(planRoute);

    }

    public void UpdateRoute(long planId, decimal? FlatPricePerCall, string? TieredPricingJson, bool IsFree)
    {
        var planRoute = PlanRoutes.FirstOrDefault(pr => pr.PlanId == planId);
        if (planRoute == null)
            throw new KeyNotFoundException("PlanRoute not found");



        planRoute.FlatPricePerCall = FlatPricePerCall;
        planRoute.TieredPricingJson = TieredPricingJson;
        planRoute.IsFree = IsFree;
        planRoute.UpdatedAt = DateTime.UtcNow;
    }

    public void deleteRoute(long planRouteId)
    {
        var planRoute = PlanRoutes.FirstOrDefault(s => s.Id == planRouteId)
            ?? throw new KeyNotFoundException("Plan Route not found");
        PlanRoutes.Remove(planRoute);
    }
}
