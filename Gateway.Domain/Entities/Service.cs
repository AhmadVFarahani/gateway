namespace Gateway.Domain.Entities;

public class Service:BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string Description { get; set; }=string.Empty;


    public ICollection<Route> Routes { get; set; } = new List<Route>();
}
