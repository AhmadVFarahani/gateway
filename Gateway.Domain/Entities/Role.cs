namespace Gateway.Domain.Entities;

public class Role : BaseEntity
{

    public string Name { get; set; } = default!; // e.g., "Admin", "Developer", etc.

    public string? Description { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
}
