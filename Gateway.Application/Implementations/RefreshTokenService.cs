using Gateway.Application.Interfaces;
using Gateway.Application.RefreshTokens.Dtos;
using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using System.Security.Cryptography;

namespace Gateway.Application.Implementations;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _repository;

    public RefreshTokenService(IRefreshTokenRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> CreateAsync(CreateRefreshTokenRequest request)
    {
        if (request.UserId == null && request.ApiKeyId == null)
            throw new ArgumentException("Either UserId or ApiKeyId must be provided.");

        var token = GenerateSecureToken();

        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = request.UserId,
            ApiKeyId = request.ApiKeyId,
            ExpirationDate = DateTime.UtcNow.AddDays(30)
        };

        await _repository.AddAsync(refreshToken);

        return token;
    }

    public async Task<RefreshTokenDto?> GetByTokenAsync(string token)
    {
        var refreshToken = await _repository.GetByTokenAsync(token);
        return refreshToken == null ? null : new RefreshTokenDto
        {
            Id = refreshToken.Id,
            UserId = refreshToken.UserId,
            ApiKeyId = refreshToken.ApiKeyId,
            Token = refreshToken.Token,
            ExpirationDate = refreshToken.ExpirationDate,
            IsRevoked = refreshToken.IsRevoked
        };
    }

    public async Task RevokeAsync(string token)
    {
        var refreshToken = await _repository.GetByTokenAsync(token)
            ?? throw new KeyNotFoundException("RefreshToken not found");

        refreshToken.IsRevoked = true;
        await _repository.UpdateAsync(refreshToken);
    }

    private static string GenerateSecureToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}