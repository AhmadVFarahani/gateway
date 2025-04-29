using Gateway.Application.Base;

namespace Gateway.Application.RouteScopes.Dtos;

public class RouteScopeDto:BaseDto
{
    public long RouteId { get; set; }
    public long ScopeId { get; set; }
}
