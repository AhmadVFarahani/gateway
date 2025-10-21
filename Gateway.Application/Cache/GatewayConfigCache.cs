using Gateway.Application.AccessPolicies.Dtos;
using Gateway.Application.ApiKeys;
using Gateway.Application.Routes.Dtos;
using Gateway.Application.RouteScopes.Dtos;
using Gateway.Application.Scopes;
using Gateway.Application.Users;

namespace Gateway.Application.Cache;

public class GatewayConfigCache
{
    public List<UserDto> Users { get; set; } = [];
    public List<ApiKeyDto> ApiKeys { get; set; } = [];
    public List<AccessPolicyDto> AccessPolicies { get; set; } = [];
    public List<ScopeDto> Scopes { get; set; } = [];
    public List<RouteDto> Routes { get; set; } = [];
    public List<RouteScopeDto> RouteScopes { get; set; } = [];
}
