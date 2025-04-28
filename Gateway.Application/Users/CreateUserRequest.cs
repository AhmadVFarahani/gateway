using Gateway.Domain.Enums;

namespace Gateway.Application.Users;

public class CreateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserType UserType { get; set; }
}
