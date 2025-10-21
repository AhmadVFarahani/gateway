namespace Gateway.Application.Interfaces.Cache;

public interface ICacheRefresher
{
    Task RefreshAuthorizationCacheAsync(CancellationToken ct = default);
    Task RefreshBusinessCacheAsync(CancellationToken ct = default);
}