using Gateway.Domain.Enumsک;

namespace Gateway.Application.Routes.Dtos;

public class CreateRouteRequest
{
    public string Path { get; set; } = string.Empty;
    public HttpMethodEnum HttpMethod { get; set; }
    public string TargetPath { get; set; } = string.Empty;
    public long ServiceId { get; set; }
    public bool RequiresAuthentication { get; set; } = true;
}
