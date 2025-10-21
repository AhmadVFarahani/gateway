using Gateway.Application.Interfaces.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Gateway.Application.Implementations.Cache;

public class HybridCacheService : IHybridCacheService
{
    private readonly IMemoryCache _memory;
    private readonly IDistributedCache _redis;
    private readonly ILogger<HybridCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOpts = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(10);
    private readonly string _instancePrefix;

    public HybridCacheService(
        IMemoryCache memory,
        IDistributedCache redis,
        ILogger<HybridCacheService> logger,
        IConfiguration cfg)
    {
        _memory = memory;
        _redis = redis;
        _logger = logger;
        _instancePrefix = cfg.GetSection("Cache")?["InstanceName"] ?? "AkamGateway:";
    }

    private string Key(string raw) => $"{_instancePrefix}{raw}";

    public async Task<T?> GetAsync<T>(string key)
    {
        var k = Key(key);

        // 1) Memory
        if (_memory.TryGetValue(k, out T? memVal))
            return memVal;

        // 2) Redis
        try
        {
            var s = await _redis.GetStringAsync(k);
            if (s is not null)
            {
                var val = JsonSerializer.Deserialize<T>(s, _jsonOpts);
                // backfill memory (shorter TTL for in-proc stability)
                _memory.Set(k, val, TimeSpan.FromMinutes(3));
                return val;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis get failed for {Key}", k);
        }
        return default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
    {
        var k = Key(key);
        var memTtl = TimeSpan.FromMinutes(Math.Max(1, (int)(ttl ?? DefaultTtl).TotalMinutes / 3)); // memory shorter

        // 1) Memory
        _memory.Set(k, value, memTtl);

        // 2) Redis
        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOpts);
            var opts = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl ?? DefaultTtl
            };
            await _redis.SetStringAsync(k, json, opts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis set failed for {Key}", k);
        }
    }

    public async Task RemoveAsync(string key)
    {
        var k = Key(key);
        _memory.Remove(k);
        try
        {
            await _redis.RemoveAsync(k);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis remove failed for {Key}", k);
        }
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? ttl = null)
    {
        var existing = await GetAsync<T>(key);
        if (existing is not null) return existing;

        var loaded = await factory();
        if (loaded is null) return default;

        await SetAsync(key, loaded, ttl);
        return loaded;
    }

    public async Task PublishRefreshAsync(string keyOrPattern)
    {
        // publish via Redis Pub/Sub (handled by subscriber service)
        try
        {
            // We’ll push a simple string payload: the logical cache key or a pattern.
            // Subscriber will interpret and invalidate + refill.
            var mux = (StackExchange.Redis.IConnectionMultiplexer)typeof(IDistributedCache)
                .Assembly
                .GetType("Microsoft.Extensions.Caching.StackExchangeRedis.RedisCache")!
                .GetField("_connection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .GetValue(_redis)!;

            var sub = mux.GetSubscriber();
            await sub.PublishAsync("cache.refresh", keyOrPattern);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Publish refresh failed for pattern {Pattern}", keyOrPattern);
        }
    }
}