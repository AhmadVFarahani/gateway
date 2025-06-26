using Gateway.Domain.Enums;

namespace Gateway.Application.Users;

public class CreateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public long CompanyId { get; set; }
    public long RoleId { get; set; }
    public UserType UserType { get; set; }
}
