namespace Gateway.Domain.Entities;

public class RefreshToken:BaseEntity
{

    public long? UserId { get; set; }
    public User? User { get; set; }

    public long? ApiKeyId { get; set; }
    public ApiKey? ApiKey { get; set; }

    public string Token { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public bool IsRevoked { get; set; } = false;
}