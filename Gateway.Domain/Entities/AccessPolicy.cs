namespace Gateway.Domain.Entities;

public class AccessPolicy:BaseEntity
{

    public long? UserId { get; set; }
    public User? User { get; set; } 

    public long? ApiKeyId { get; set; }
    public ApiKey? ApiKey { get; set; } 

    public long ScopeId { get; set; }
    public Scope Scope { get; set; } = null!;

    public bool IsActive { get; set; } = true;

}
