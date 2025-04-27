using AutoMapper;
using Gateway.Application.Interfaces;
using Gateway.Application.Services.Dtos;
using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;

namespace Gateway.Application.Implementations;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _repository;
    private readonly IMapper _mapper;

    public ServiceService(IServiceRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ServiceDto?> GetByIdAsync(long id)
    {
        var service = await _repository.GetByIdAsync(id);
        return service == null ? null : _mapper.Map<ServiceDto>(service);
    }

    public async Task<IEnumerable<ServiceDto>> GetAllAsync()
    {
        var services = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ServiceDto>>(services);
    }

    public async Task<long> CreateAsync(CreateServiceRequest request)
    {
        var service = new Service
        {
            Name = request.Name,
            BaseUrl = request.BaseUrl
        };

        await _repository.AddAsync(service);
        return service.Id;
    }

    public async Task UpdateAsync(long id, UpdateServiceRequest request)
    {
        var service = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Service not found");

        service.Name = request.Name;
        service.BaseUrl = request.BaseUrl;
        service.IsActive = request.IsActive;
        service.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(service);
    }

    public async Task DeleteAsync(long id)
    {
        var service = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Service not found");

        await _repository.DeleteAsync(service);
    }
}
