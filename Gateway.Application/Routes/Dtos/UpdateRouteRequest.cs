using Gateway.Domain.Enums;

namespace Gateway.Application.Routes.Dtos;

public class UpdateRouteRequest
{
    public string Path { get; set; } = string.Empty;
    public HttpMethodEnum HttpMethod { get; set; }
    public string TargetPath { get; set; } = string.Empty;
    public bool RequiresAuthentication { get; set; } = true;
    public bool IsActive { get; set; } = true;
}