using AutoMapper;
using Gateway.Application.AccessPolicies.Dtos;
using Gateway.Application.ApiKeys;
using Gateway.Application.Cache;
using Gateway.Application.Contract.Dtos;
using Gateway.Application.Interfaces.Cache;
using Gateway.Application.Plan.Dtos;
using Gateway.Application.Routes.Dtos;
using Gateway.Application.RouteScopes.Dtos;
using Gateway.Application.Scopes;
using Gateway.Application.Users;
using Gateway.Domain.Interfaces;

namespace Gateway.Application.Implementations.Cache;

public class CacheLoader : ICacheLoader
{
    private readonly IUserRepository _users;
    private readonly IApiKeyRepository _keys;
    private readonly IScopeRepository _scopes;
    private readonly IRouteRepository _routes;
    private readonly IContractRepository _contracts;
    private readonly IPlanRepository _plans;
    private readonly IAccessPolicyRepository _policies;
    private readonly IRouteScopeRepository _routeScopes;
    private readonly IPlanRouteRepository _planRoute;
    private readonly IMapper _mapper;

    public CacheLoader(IUserRepository users, IApiKeyRepository keys, IScopeRepository scopes, IRouteRepository routes, IContractRepository contracts, IPlanRepository plans, IMapper mapper, IAccessPolicyRepository policies, IRouteScopeRepository routeScopes, IPlanRouteRepository planRoute)
    {
        _users = users;
        _keys = keys;
        _scopes = scopes;
        _routes = routes;
        _contracts = contracts;
        _plans = plans;
        _mapper = mapper;
        _policies = policies;
        _routeScopes = routeScopes;
        _planRoute = planRoute;
    }

    public async Task<GatewayConfigCache> LoadAuthorizationDataAsync(CancellationToken ct = default)
    {
        var cache = new GatewayConfigCache();

        var users = await _users.GetAllAsync();
        cache.Users = _mapper.Map<IEnumerable<UserDto>>(users).ToList();

        var apiKeys = await _keys.GetAllAsync();
        cache.ApiKeys = _mapper.Map<IEnumerable<ApiKeyDto>>(apiKeys).ToList();

        var accessPolicies = await _policies.GetAllAsync();
        cache.AccessPolicies = _mapper.Map<IEnumerable<AccessPolicyDto>>(accessPolicies).ToList();

        var scopes = await _scopes.GetAllAsync();
        cache.Scopes = _mapper.Map<IEnumerable<ScopeDto>>(scopes).ToList();

        var routes = await _routes.GetAllAsync();
        cache.Routes = _mapper.Map<IEnumerable<RouteDto>>(routes).ToList();

        var routeScopes = await _routeScopes.GetAllAsync();
        cache.RouteScopes = _mapper.Map<IEnumerable<RouteScopeDto>>(routeScopes).ToList();

        return cache;
    }

    public async Task<GatewayPlanCache> LoadBusinessDataAsync(CancellationToken ct = default)
    {
        var cache = new GatewayPlanCache();

        var contracts = await _contracts.GetAllAsync();
        cache.Contracts = _mapper.Map<IEnumerable<ContractDto>>(contracts).ToList();

        var plans = await _plans.GetAllAsync();
        cache.Plans = _mapper.Map<IEnumerable<PlanDto>>(plans).ToList();

        var planRoutes = await _planRoute.GetAllAsync();
        cache.PlanRoutes = _mapper.Map<IEnumerable<PlanRouteDto>>(planRoutes).ToList();

        return cache;
    }
}
