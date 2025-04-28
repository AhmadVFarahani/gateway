using Gateway.Domain.Enums;

namespace Gateway.Application.Users;

public class UpdateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public UserType UserType { get; set; }
}