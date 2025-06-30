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
}
