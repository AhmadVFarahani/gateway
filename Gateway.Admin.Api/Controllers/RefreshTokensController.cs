using Gateway.Application.Interfaces;
using Gateway.Application.RefreshTokens.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RefreshTokensController : ControllerBase
{
    private readonly IRefreshTokenService _service;

    public RefreshTokensController(IRefreshTokenService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRefreshTokenRequest request)
    {
        var token = await _service.CreateAsync(request);
        return Ok(new { RefreshToken = token });
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] string token)
    {
        await _service.RevokeAsync(token);
        return NoContent();
    }
}