namespace Gateway.Domain.Entities;

public class RouteResponseField : BaseEntity
{
    public long RouteId { get; set; }

    public string Name { get; set; } = null!;
    public string? Type { get; set; } // e.g., string, int, object
    public string? Description { get; set; }
    public bool IsRequired { get; set; }
    public long? ParentId { get; set; }

    // Navigation
    public Route Route { get; set; } = null!;
    public RouteResponseField? Parent { get; set; }
    public ICollection<RouteResponseField> Children { get; set; } = new List<RouteResponseField>();
}