using Gateway.Application.AccessPolicies.Dtos;
using Gateway.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccessPoliciesController : ControllerBase
{
    private readonly IAccessPolicyService _service;

    public AccessPoliciesController(IAccessPolicyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccessPolicyRequest request)
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
