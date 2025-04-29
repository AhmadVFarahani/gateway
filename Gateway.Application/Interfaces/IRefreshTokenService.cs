using Gateway.Application.RefreshTokens.Dtos;

namespace Gateway.Application.Interfaces;

public interface IRefreshTokenService
{
    Task<string> CreateAsync(CreateRefreshTokenRequest request);
    Task<RefreshTokenDto?> GetByTokenAsync(string token);
    Task RevokeAsync(string token);
}