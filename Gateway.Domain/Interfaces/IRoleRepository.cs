using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(long id);
    Task<IEnumerable<Role>> GetAllAsync();
    Task AddAsync(Role role);
    Task UpdateAsync(Role role);
    Task DeleteAsync(Role role);
}
