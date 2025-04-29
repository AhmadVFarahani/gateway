using Gateway.Application.RouteScopes.Dtos;

namespace Gateway.Application.Interfaces;

public interface IRouteScopeService
{
    Task<IEnumerable<RouteScopeDto>> GetAllAsync();
    Task<long> CreateAsync(CreateRouteScopeRequest request);
    Task DeleteAsync(long id);
}