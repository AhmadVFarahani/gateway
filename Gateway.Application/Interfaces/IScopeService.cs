using Gateway.Application.Scopes;

namespace Gateway.Application.Interfaces;

public interface IScopeService
{
    Task<ScopeDto?> GetByIdAsync(long id);
    Task<IEnumerable<ScopeDto>> GetAllAsync();
    Task<long> CreateAsync(CreateScopeRequest request);
    Task UpdateAsync(long id, UpdateScopeRequest request);
    Task DeleteAsync(long id);
}