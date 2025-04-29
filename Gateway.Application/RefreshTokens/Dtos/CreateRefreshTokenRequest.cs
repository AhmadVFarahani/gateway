namespace Gateway.Application.RefreshTokens.Dtos;

public class CreateRefreshTokenRequest
{
    public long? UserId { get; set; }
    public long? ApiKeyId { get; set; }
}