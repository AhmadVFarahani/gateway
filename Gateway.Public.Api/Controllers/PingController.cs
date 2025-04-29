using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Public.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PingController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult PublicPing() => Ok("✅ Public Access");

    [Authorize]
    [HttpGet("secure")]
    public IActionResult SecurePing() => Ok("🔐 Authorized Access");
}