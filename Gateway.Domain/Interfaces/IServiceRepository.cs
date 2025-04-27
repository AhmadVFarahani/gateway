using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(long id);
    Task<IEnumerable<Service>> GetAllAsync();
    Task AddAsync(Service service);
    Task UpdateAsync(Service service);
    Task DeleteAsync(Service service);
}
