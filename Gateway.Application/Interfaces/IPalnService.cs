using Gateway.Application.Plan.Dtos;
using Gateway.Application.Plan.Dtos;

namespace Gateway.Application.Interfaces;

public interface IPlanService
{
    Task<PlanDto?> GetByIdAsync(long id);
    Task<IEnumerable<PlanDto>> GetAllAsync();
    Task<long> CreateAsync(CreatePlanRequest request);
    Task UpdateAsync(long id, UpdatePlanRequest request);
    Task DeleteAsync(long id);
}