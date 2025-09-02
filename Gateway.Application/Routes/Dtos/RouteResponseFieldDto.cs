namespace Gateway.Application.Routes.Dtos;

public class RouteResponseFieldDto
{
    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsRequired { get; set; }
    public List<RouteResponseFieldDto>? Children { get; set; }


}

public class RouteRequestFieldDto
{
    public string FieldName { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string? Format { get; set; }               
    public bool IsRequired { get; set; }
    public string? Description { get; set; }

    public List<RouteRequestFieldDto>? Children { get; set; }
}
