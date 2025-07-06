using System.ComponentModel.Design;

namespace Gateway.Domain.Entities;

public class Scope : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<RouteScope> RouteScopes { get; set; } = new List<RouteScope>();
    public ICollection<AccessPolicy> AccessPolicies { get; set; } = new List<AccessPolicy>();



    public void addRoute(RouteScope route)
    {
        RouteScopes.Add(route);
    }
    public void deleteRoute(long routeId)
    {
        var route = RouteScopes.FirstOrDefault(s => s.Id == routeId)
            ?? throw new KeyNotFoundException("route not found");
        RouteScopes.Remove(route);
    }
}