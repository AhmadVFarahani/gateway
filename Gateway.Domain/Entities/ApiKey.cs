namespace Gateway.Domain.Entities;

public class ApiKey:BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public long UserId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ExpirationDate { get; set; }

    public User? User { get; set; }

    public ICollection<AccessPolicy> AccessPolicies { get; set; } = new List<AccessPolicy>();
}