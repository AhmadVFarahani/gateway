using Gateway.Application.Base;
namespace Gateway.Application.Role.Dtos;

public class RoleDto:BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description{ get; set; } = string.Empty;
}
