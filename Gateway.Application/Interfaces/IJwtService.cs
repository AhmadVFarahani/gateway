using Gateway.Domain.Entities;

namespace Gateway.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user, ApiKey apiKey);
}