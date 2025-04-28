using Gateway.Domain.Enums;

namespace Gateway.Domain.Entities;

public class User:BaseEntity
{
    
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public UserType UserType { get; set; } = UserType.Admin;
}