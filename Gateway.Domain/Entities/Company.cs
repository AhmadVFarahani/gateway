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




    public void addPlan(CompanyPlan plan)
    {
        CompanyPlans.Add(plan);
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
        CompanyPlans.Add(plan);
    }
}
