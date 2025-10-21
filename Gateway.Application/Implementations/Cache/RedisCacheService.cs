using Gateway.Application.Interfaces.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Gateway.Application.Implementations.Cache;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var data = await _cache.GetStringAsync(key);
            if (data == null)
                return default;

            return JsonSerializer.Deserialize<T>(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get key {Key} from Redis", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions();

            if (expiry != null)
                options.AbsoluteExpirationRelativeToNow = expiry;

            await _cache.SetStringAsync(key, json, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set key {Key} in Redis", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove key {Key} from Redis", key);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var value = await _cache.GetAsync(key);
            return value != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check existence for key {Key}", key);
            return false;
        }
    }
}
