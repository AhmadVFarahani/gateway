namespace Gateway.Application.Services.Dtos;

public class CreateServiceRequest
{
    public string Name { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string Description { get; set; }
}
