using Gateway.Application.Routes.Dtos;

namespace Gateway.Application.Interfaces;

public interface IRouteService
{
    Task<RouteDto?> GetByIdAsync(long id);
    Task<IEnumerable<RouteDto>> GetAllAsync();
    Task<long> CreateAsync(CreateRouteRequest request);
    Task UpdateAsync(long id, UpdateRouteRequest request);
    Task DeleteAsync(long id);
}