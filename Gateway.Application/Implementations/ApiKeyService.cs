using AutoMapper;
using Gateway.Application.ApiKeys;
using Gateway.Application.Interfaces;
using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using System.Security.Cryptography;

namespace Gateway.Application.Implementations;

public class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyRepository _repository;
    private readonly IMapper _mapper;

    public ApiKeyService(IApiKeyRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiKeyDto?> GetByKeyAsync(string key)
    {
        var apiKey = await _repository.GetByKeyAsync(key);
        return apiKey == null ? null : _mapper.Map<ApiKeyDto>(apiKey);
    }

    public async Task<IEnumerable<ApiKeyDto>> GetAllAsync()
    {
        var keys = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ApiKeyDto>>(keys);
    }

    public async Task<string> CreateAsync(CreateApiKeyRequest request)
    {
        var key = GenerateApiKey();

        var apiKey = new ApiKey
        {
            Key = key,
            UserId = request.UserId,
            ExpirationDate = request.ExpirationDate,
            IsActive = true
        };

        await _repository.AddAsync(apiKey);

        return key;
    }

    public async Task DeleteAsync(long id)
    {
        var apiKey = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("API Key not found");

        await _repository.DeleteAsync(apiKey);
    }

    private static string GenerateApiKey()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
