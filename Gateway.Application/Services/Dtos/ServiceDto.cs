using Gateway.Application.Base;

namespace Gateway.Application.Services.Dtos;

public class ServiceDto:BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
