namespace Gateway.Domain.Entities;

public class RouteRequestField:BaseEntity
{
    // Foreign Key to Route
    public long RouteId { get; set; }
    public Route Route { get; set; } = default!;

    // Field Info
    public string FieldName { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string? Format { get; set; }
    public bool IsRequired { get; set; }
    public string? Description { get; set; }

    // Self-reference
    public long? ParentId { get; set; }
    public RouteRequestField? Parent { get; set; }
    public ICollection<RouteRequestField>? Children { get; set; }
}
