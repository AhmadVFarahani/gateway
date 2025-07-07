using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(long id);
    Task<IEnumerable<Plan>> GetAllAsync();
    Task AddAsync(Plan plan);
    Task UpdateAsync(Plan plan);
    Task DeleteAsync(Plan plan);


    Task<Plan?> GetWithRouteAsync(long id);
    Task<PlanRoute?> GetPlanRouteByIdAsync(long planId, long planRouteId);

}
