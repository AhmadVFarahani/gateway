using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(long id);
    Task<IEnumerable<Plan>> GetAllAsync();
    Task AddAsync(Plan plan);
    Task UpdateAsync(Plan plan);
    Task DeleteAsync(Plan plan);
}
