using Gateway.Application.Base;
namespace Gateway.Application.Company.Dtos;

public class CompanyDto:BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description{ get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
