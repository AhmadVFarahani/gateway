using Gateway.Application.Routes.Dtos;

namespace Gateway.Application.Interfaces;

public interface IRouteService
{
    Task<RouteDto?> GetByIdAsync(long id);
    Task<IEnumerable<RouteDto>> GetAllAsync();
    Task<IEnumerable<RouteDto>> GetByServiceId(long serviceId);
    Task<long> CreateAsync(CreateRouteRequest request);
    Task UpdateAsync(long id, UpdateRouteRequest request);
    Task DeleteAsync(long id);
    Task<byte[]> ExportToExcel();
    Task SaveResponseFieldsAsync(long routeId, List<RouteResponseFieldDto> fields);
    Task SaveRequestFieldsAsync(long routeId, List<RouteRequestFieldDto> fields);

    Task<List<RouteRequestFieldDto>> GetRequestSchemaAsync(long routeId);
    Task<List<RouteResponseFieldDto>> GetResponseSchemaAsync(long routeId);
}
