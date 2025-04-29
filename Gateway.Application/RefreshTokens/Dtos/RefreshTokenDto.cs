using Gateway.Application.Base;

namespace Gateway.Application.RefreshTokens.Dtos;

public class RefreshTokenDto:BaseDto
{
    public long? UserId { get; set; }
    public long? ApiKeyId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public bool IsRevoked { get; set; }
}
