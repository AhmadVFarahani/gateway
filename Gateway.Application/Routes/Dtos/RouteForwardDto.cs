using Gateway.Domain.Enums;

namespace Gateway.Application.Routes.Dtos;

public class RouteForwardDto
{
    public long Id { get; set; }
    public string Path { get; set; } = string.Empty;  // e.g. "/api/company/{id}"
    public HttpMethodEnum HttpMethod { get; set; }
    public string TargetPath { get; set; } = string.Empty; // e.g. "/internal/company/{id}"
    public long ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string TargetBaseUrl { get; set; } = "";     // e.g. "http://localhost:6001"
    public string TargetCluster { get; set; } = "default"; // grouping key
}
