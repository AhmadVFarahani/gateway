using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IRouteScopeRepository
{
    Task<IEnumerable<RouteScope>> GetAllAsync();
    Task AddAsync(RouteScope routeScope);
    Task DeleteAsync(RouteScope routeScope);
    Task<RouteScope?> GetByIdAsync(long id);
}