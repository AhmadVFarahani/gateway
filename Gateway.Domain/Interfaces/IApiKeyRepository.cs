using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IApiKeyRepository
{
    Task<ApiKey?> GetByKeyAsync(string key);
    Task<ApiKey?> GetByIdAsync(long id);
    Task<IEnumerable<ApiKey>> GetAllAsync();
    Task AddAsync(ApiKey apiKey);
    Task DeleteAsync(ApiKey apiKey);
}