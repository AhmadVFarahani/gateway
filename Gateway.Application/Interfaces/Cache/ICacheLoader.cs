using Gateway.Application.Cache;

namespace Gateway.Application.Interfaces.Cache;

public interface ICacheLoader
{
    Task<GatewayConfigCache> LoadAuthorizationDataAsync(CancellationToken ct = default);
    Task<GatewayPlanCache> LoadBusinessDataAsync(CancellationToken ct = default);
}