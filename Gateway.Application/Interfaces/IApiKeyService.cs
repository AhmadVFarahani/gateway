using Gateway.Application.ApiKeys;

namespace Gateway.Application.Interfaces;

public interface IApiKeyService
{
    Task<ApiKeyDto?> GetByKeyAsync(string key);
    Task<IEnumerable<ApiKeyDto>> GetAllAsync();
    Task<string> CreateAsync(CreateApiKeyRequest request);
    Task DeleteAsync(long id);
}