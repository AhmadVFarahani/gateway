namespace Gateway.Application.ApiKeys;

public class CreateApiKeyRequest
{
    public long UserId { get; set; }
    public DateTime? ExpirationDate { get; set; }
}
