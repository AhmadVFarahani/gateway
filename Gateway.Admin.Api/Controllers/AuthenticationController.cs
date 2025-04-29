using Gateway.Application.Authentication.Dtos;
using Gateway.Application.Interfaces;
using Gateway.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Admin.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthenticationController(
        IApiKeyRepository apiKeyRepository,
        IUserRepository userRepository,
        IJwtService jwtService)
    {
        _apiKeyRepository = apiKeyRepository;
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    [HttpPost("token")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ApiKey))
            return BadRequest("API Key is required.");

        var apiKey = await _apiKeyRepository.GetByKeyAsync(request.ApiKey);
        if (apiKey == null || !apiKey.IsActive || (apiKey.ExpirationDate.HasValue && apiKey.ExpirationDate.Value < DateTime.UtcNow))
            return Unauthorized("Invalid or expired API Key.");

        var user = await _userRepository.GetByIdAsync(apiKey.UserId);
        if (user == null || !user.IsActive)
            return Unauthorized("User is inactive.");

        var token = _jwtService.GenerateToken(user, apiKey);

        var expiration = DateTime.UtcNow.AddMinutes(int.Parse(
            HttpContext.RequestServices.GetRequiredService<IConfiguration>()["JwtSettings:ExpiresInMinutes"] ?? "30"));

        return Ok(new AuthenticateResponse
        {
            Token = token,
            Expiration = expiration
        });
    }
}