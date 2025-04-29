using AutoMapper;
using Gateway.Application.Interfaces;
using Gateway.Application.Scopes;
using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;

namespace Gateway.Application.Implementations;

public class ScopeService : IScopeService
{
    private readonly IScopeRepository _repository;
    private readonly IMapper _mapper;

    public ScopeService(IScopeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ScopeDto?> GetByIdAsync(long id)
    {
        var scope = await _repository.GetByIdAsync(id);
        return scope == null ? null : _mapper.Map<ScopeDto>(scope);
    }

    public async Task<IEnumerable<ScopeDto>> GetAllAsync()
    {
        var scopes = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ScopeDto>>(scopes);
    }

    public async Task<long> CreateAsync(CreateScopeRequest request)
    {
        var scope = new Scope
        {
            Name = request.Name,
            DisplayName = request.DisplayName,
            Description = request.Description,
            IsActive = true
        };

        await _repository.AddAsync(scope);
        return scope.Id;
    }

    public async Task UpdateAsync(long id, UpdateScopeRequest request)
    {
        var scope = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Scope not found");

        scope.DisplayName = request.DisplayName;
        scope.Description = request.Description;
        scope.IsActive = request.IsActive;
        scope.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(scope);
    }

    public async Task DeleteAsync(long id)
    {
        var scope = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Scope not found");

        await _repository.DeleteAsync(scope);
    }
}
