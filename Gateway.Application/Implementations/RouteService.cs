using AutoMapper;
using ClosedXML.Excel;
using Gateway.Application.Interfaces;
using Gateway.Application.Routes.Dtos;
using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;

namespace Gateway.Application.Implementations;

public class RouteService : IRouteService
{
    private readonly IRouteRepository _repository;
    private readonly IMapper _mapper;

    public RouteService(IRouteRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<RouteDto?> GetByIdAsync(long id)
    {
        var route = await _repository.GetByIdAsync(id);
        return route == null ? null : _mapper.Map<RouteDto>(route);
    }

    public async Task<IEnumerable<RouteDto>> GetAllAsync()
    {
        var routes = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<RouteDto>>(routes);
    }
    public async Task<IEnumerable<RouteDto>> GetByServiceId(long serviceId)
    {
        var routes = await _repository.GetByServiceId(serviceId);
        return _mapper.Map<IEnumerable<RouteDto>>(routes);
    }

    public async Task<long> CreateAsync(CreateRouteRequest request)
    {
        var route = new Route
        {
            Path = request.Path,
            HttpMethod = request.HttpMethod,
            TargetPath = request.TargetPath,
            ServiceId = request.ServiceId,
            RequiresAuthentication = request.RequiresAuthentication,
        };

        await _repository.AddAsync(route);
        return route.Id;
    }

    public async Task UpdateAsync(long id, UpdateRouteRequest request)
    {
        var route = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Route not found");

        route.Path = request.Path;
        route.HttpMethod = request.HttpMethod;
        route.TargetPath = request.TargetPath;
        route.RequiresAuthentication = request.RequiresAuthentication;
        route.IsActive = request.IsActive;
        route.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(route);
    }

    public async Task DeleteAsync(long id)
    {
        var route = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Route not found");

        await _repository.DeleteAsync(route);
    }
    public async Task<byte[]> ExportToExcel()
    {
        var routes = (await _repository.GetAllAsync()).ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Invoices");

        // Header
        worksheet.Cell(1, 1).Value = "Path";
        worksheet.Cell(1, 2).Value = "HttpMethod";
        worksheet.Cell(1, 3).Value = "TargetPath";
        worksheet.Cell(1, 4).Value = "ServiceId";
        worksheet.Cell(1, 5).Value = "RequiresAuthentication";
        worksheet.Cell(1, 6).Value = "IsActive";

        // Data
        for (int i = 0; i < routes.Count; i++)
        {
            var row = i + 2;
            var inv = routes[i];

            worksheet.Cell(row, 1).Value = inv.Path;
            worksheet.Cell(row, 2).Value = inv.HttpMethod.ToString();
            worksheet.Cell(row, 3).Value = inv.TargetPath;
            worksheet.Cell(row, 4).Value = inv.ServiceId;
            worksheet.Cell(row, 5).Value = inv.RequiresAuthentication;
            worksheet.Cell(row, 6).Value = inv.IsActive;
        }

        // Auto-size columns
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<List<RouteRequestFieldDto>> GetRequestSchemaAsync(long routeId)
    {
        var route = await _repository.GetByIdWithFieldsAsync(routeId)
           ?? throw new KeyNotFoundException("Route not found");

        return BuildRequestTree(route.RequestFields.ToList(), parentId: null);
    }

    private List<RouteRequestFieldDto> BuildRequestTree(List<RouteRequestField> rows, long? parentId)
    {
        return rows
            .Where(x => x.ParentId == parentId)
            .Select(x => new RouteRequestFieldDto
            {
                FieldName = x.FieldName,
                Type = x.Type,
                Format = x.Format,
                IsRequired = x.IsRequired,
                Description = x.Description,
                Children = BuildRequestTree(rows, x.Id)
            })
            .ToList();
    }

    // ---------- RESPONSE ----------
    public async Task<List<RouteResponseFieldDto>> GetResponseSchemaAsync(long routeId)
    {
        var route = await _repository.GetByIdWithFieldsAsync(routeId)
           ?? throw new KeyNotFoundException("Route not found");
        return BuildResponseTree(route.ResponseFields.ToList(), parentId: null);
    }

    private List<RouteResponseFieldDto> BuildResponseTree(List<RouteResponseField> rows, long? parentId)
    {
        return rows
            .Where(x => x.ParentId == parentId)
            .Select(x => new RouteResponseFieldDto
            {
                Name = x.Name,
                Type = x.Type,
                IsRequired = x.IsRequired,
                Description = x.Description,
                Children = BuildResponseTree(rows, x.Id)
            })
            .ToList();
    }


    public async Task SaveResponseFieldsAsync(long routeId, List<RouteResponseFieldDto> fields)
    {
        var route = await _repository.GetByIdWithFieldsAsync(routeId)
           ?? throw new KeyNotFoundException("Route not found");
        foreach (var field in fields)
        {
         await AddResponseFieldRecursive(route, field, null);
        }

        
    }

    public async Task SaveRequestFieldsAsync(long routeId, List<RouteRequestFieldDto> fields)
    {
        var route = await _repository.GetByIdWithFieldsAsync(routeId)
           ?? throw new KeyNotFoundException("Route not found");
        foreach (var field in fields)
        {
            await AddRequestFieldRecursive(route, field, null);
        }


    }

    private async Task AddRequestFieldRecursive(Route route, RouteRequestFieldDto dto, long? parentId)
    {
        var entity = new RouteRequestField
        {
            RouteId = route.Id,
            FieldName = dto.FieldName,
            Type = dto.Type,
            Description = dto.Description,
            IsRequired = dto.IsRequired,
            ParentId = parentId,
        };

        route.addRequestFields(entity);
        await _repository.UpdateAsync(route);

        if (dto.Children != null && dto.Children.Any())
        {
            foreach (var child in dto.Children)
            {
             await AddRequestFieldRecursive(route, child, entity.Id);
            }
        }
    }

    private async Task AddResponseFieldRecursive(Route route, RouteResponseFieldDto dto, long? parentId)
    {
        var entity = new RouteResponseField
        {
            RouteId = route.Id,
            Name = dto.Name,
            Type = dto.Type,
            Description = dto.Description,
            IsRequired = dto.IsRequired,
            ParentId = parentId,
        };

        route.addResponseFields(entity);
        await _repository.UpdateAsync(route);

        if (dto.Children != null && dto.Children.Any())
        {
            foreach (var child in dto.Children)
            {
                await AddResponseFieldRecursive(route, child, entity.Id);
            }
        }
    }
}