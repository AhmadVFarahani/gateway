using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IScopeRepository
{
    Task<Scope?> GetByIdAsync(long id);
    Task<IEnumerable<Scope>> GetAllAsync();
    Task AddAsync(Scope scope);
    Task UpdateAsync(Scope scope);
    Task DeleteAsync(Scope scope);
}