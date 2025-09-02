using Gateway.Domain.Enums;

namespace Gateway.Domain.Entities;

public class Route:BaseEntity
{
    public string Path { get; set; } = string.Empty;
    public HttpMethodEnum HttpMethod { get; set; } = HttpMethodEnum.GET;
    public string TargetPath { get; set; } = string.Empty;
    public long ServiceId { get; set; }
    public bool RequiresAuthentication { get; set; } = true;
    public bool IsActive { get; set; } = true;
  
    public Service? Service { get; set; }
    public ICollection<RouteRequestField> RequestFields { get; set; } = new List<RouteRequestField>();
    public ICollection<RouteResponseField> ResponseFields { get; set; } = new List<RouteResponseField>();

    public ICollection<PlanRoute> PlanRoutes { get; set; } = new List<PlanRoute>();


    public void addResponseFields(RouteResponseField response)
    {
        ResponseFields.Add(response);
    }

    public void addRequestFields(RouteRequestField request)
    {
        RequestFields.Add(request);
    }

}
