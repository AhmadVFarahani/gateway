namespace Gateway.Domain.Entities;

public class RouteScope:BaseEntity
{
    public long RouteId { get; set; }
    public Route Route { get; set; } = null!;

    public long ScopeId { get; set; }
    public Scope Scope { get; set; } = null!;
}