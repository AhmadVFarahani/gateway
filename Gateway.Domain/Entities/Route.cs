namespace Gateway.Domain.Entities;

public class Route:BaseEntity
{
    public long Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public string TargetPath { get; set; } = string.Empty;
    public long ServiceId { get; set; }
    public bool RequiresAuthentication { get; set; } = true;
    public bool IsActive { get; set; } = true;
  
    public Service? Service { get; set; }
}
