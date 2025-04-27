using Gateway.Application.Services.Dtos;

namespace Gateway.Application.Interfaces;

public interface IServiceService
{
    Task<ServiceDto?> GetByIdAsync(long id);
    Task<IEnumerable<ServiceDto>> GetAllAsync();
    Task<long> CreateAsync(CreateServiceRequest request);
    Task UpdateAsync(long id, UpdateServiceRequest request);
    Task DeleteAsync(long id);
}
