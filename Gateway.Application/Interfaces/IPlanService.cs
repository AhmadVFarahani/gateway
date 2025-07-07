using Gateway.Application.Plan.Dtos;
using Gateway.Application.RouteScopes.Dtos;

namespace Gateway.Application.Interfaces;

public interface IPlanService
{
    Task<PlanDto?> GetByIdAsync(long id);
    Task<IEnumerable<PlanDto>> GetAllAsync();
    Task<long> CreateAsync(CreatePlanRequest request);
    Task UpdateAsync(long id, UpdatePlanRequest request);
    Task DeleteAsync(long id);

    #region Route
    Task<IEnumerable<PlanRouteDto>> GetPlanRouteAsync(long planId);
    Task<PlanRouteDto> GetPlanRouteByIdAsync(long planId, long planRouteId);
    Task<long> AddRouteToPlanAsync(long planId, CreatePlanRouteRequest request);
    Task UpdatePlanRouteAsync(long planId, long planRouteId, UpdatePlanRouteRequest request);
    Task DeleteRoutePlanAsync(long planId, long planRouteId);
    #endregion Route
}