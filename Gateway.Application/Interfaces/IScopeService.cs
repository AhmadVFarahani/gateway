using Gateway.Application.RouteScopes.Dtos;
using Gateway.Application.Scopes;

namespace Gateway.Application.Interfaces;

public interface IScopeService
{
    Task<ScopeDto?> GetByIdAsync(long id);
    Task<IEnumerable<ScopeDto>> GetAllAsync();
    Task<long> CreateAsync(CreateScopeRequest request);
    Task UpdateAsync(long id, UpdateScopeRequest request);
    Task DeleteAsync(long id);

    #region Route
    Task<IEnumerable<RouteScopeDto>> GetRouteScopesAsync(long scopeId);
    Task<RouteScopeDto> GetRouteScopeByIdAsync(long scopeId, long routeScopeId);
    Task<long> AddRouteToScopeAsync(long scopeId, CreateRouteScopeRequest request);
    Task DeleteRouteScopeAsync(long scopeId, long routeScopeId);
    #endregion Route
}