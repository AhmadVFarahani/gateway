using Gateway.Domain.Entities;
using Gateway.Domain.Enums;

namespace Gateway.Application.Users;

public class UserDto:BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public UserType UserType { get; set; }
}
