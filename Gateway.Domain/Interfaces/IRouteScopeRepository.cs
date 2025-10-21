using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IRouteScopeRepository
{
    Task<IEnumerable<RouteScope>> GetAllAsync();
}