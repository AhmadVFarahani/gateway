using AutoMapper;
using Gateway.Application.Interfaces;
using Gateway.Application.RouteScopes.Dtos;
using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;

namespace Gateway.Application.Implementations;

public class RouteScopeService : IRouteScopeService
{
    private readonly IRouteScopeRepository _repository;
    private readonly IMapper _mapper;

    public RouteScopeService(IRouteScopeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RouteScopeDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<RouteScopeDto>>(items);
    }

    public async Task<long> CreateAsync(CreateRouteScopeRequest request)
    {
        var entity = new RouteScope
        {
            RouteId = request.RouteId,
            ScopeId = request.ScopeId
        };

        await _repository.AddAsync(entity);
        return entity.Id;
    }

    public async Task DeleteAsync(long id)
    {
        var item = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("RouteScope not found");

        await _repository.DeleteAsync(item);
    }
}