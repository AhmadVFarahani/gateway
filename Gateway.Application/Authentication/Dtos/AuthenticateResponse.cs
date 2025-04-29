namespace Gateway.Application.Authentication.Dtos;

public class AuthenticateResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
}