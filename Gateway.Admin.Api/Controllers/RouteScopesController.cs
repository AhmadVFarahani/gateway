using Gateway.Application.Interfaces;
using Gateway.Application.RouteScopes.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RouteScopesController : ControllerBase
{
    private readonly IRouteScopeService _service;

    public RouteScopesController(IRouteScopeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRouteScopeRequest request)
    {
        var id = await _service.CreateAsync(request);
        return Ok(new { id });
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}