using Gateway.Application.Plan.Dtos;

namespace Gateway.Application.Interfaces;

public interface IPlanService
{
    Task<PlanDto?> GetByIdAsync(long id);
    Task<IEnumerable<PlanDto>> GetAllAsync();
    Task<long> CreateAsync(CreatePlanRequest request);
    Task UpdateAsync(long id, UpdatePlanRequest request);
    Task DeleteAsync(long id);


    Task<PlanRouteDto?> GetPlanRouteByIdAsync(long id);
    Task<IEnumerable<PlanRouteDto>> GetPlanRouteByPlanId(long planId);
    Task<IEnumerable<PlanRouteDto>> GetPlanRouteByRouteId(long planId);
    Task<long> CreatePlanRouteAsync(CreatePlanRouteRequest request);
    Task UpdatePlanRouteAsync(long id, UpdatePlanRouteRequest request);

}