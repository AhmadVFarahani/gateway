namespace Gateway.Application.Interfaces.Cache;

public interface IHybridCacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null);
    Task RemoveAsync(string key);

    // Sugar: tries Memory → Redis; if missing, uses factory to load from DB, then caches both.
    Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? ttl = null);

    // Pub/Sub helpers (for config changes)
    Task PublishRefreshAsync(string keyOrPattern); // e.g., "route:*" or "route:123"
}