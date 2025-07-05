using Gateway.Application.Base;

namespace Gateway.Application.RouteScopes.Dtos;

public class RouteScopeDto : BaseDto
{
    public long RouteId { get; set; }
    public string RoutePath { get; set; } = string.Empty;

    public long ScopeId { get; set; }
    public string ScopeName { get; set; } = string.Empty;

    public string ServiceName { get; set; } = string.Empty;
}
