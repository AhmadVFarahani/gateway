using AutoMapper;
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
}