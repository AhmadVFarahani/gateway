using Gateway.Application.Interfaces;
using Gateway.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Gateway.Application.Implementations;

public class JwtService: IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user, ApiKey apiKey)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new Exception("SecretKey is missing");
        var expiresInMinutes = int.Parse(_configuration["JwtSettings:ExpiresInMinutes"] ?? "30");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("ApiKeyId", apiKey.Id.ToString()),
            new Claim("CompanyId", user.CompanyId.ToString()),
            new Claim("Role", user.UserType.ToString()), // Admin or Client
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}