using Gateway.Application.Interfaces;
using Gateway.Application.Scopes;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Admin.Api.Controllers;

[Route("api/[controller]")]
public class ScopesController : ControllerBase
{
    private readonly IScopeService _service;

    public ScopesController(IScopeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var scope = await _service.GetByIdAsync(id);
        return scope == null ? NotFound() : Ok(scope);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScopeRequest request)
    {
        var id = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateScopeRequest request)
    {
        await _service.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}