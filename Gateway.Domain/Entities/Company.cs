using Gateway.Domain.Enums;

namespace Gateway.Domain.Entities;

public class Company:BaseEntity
{
    
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
}
