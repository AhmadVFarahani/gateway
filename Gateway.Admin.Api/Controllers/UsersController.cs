using Gateway.Application.Interfaces;
using Gateway.Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Admin.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());


    [HttpGet("byCompany")]
    public async Task<IActionResult> GetByCompanyId([FromQuery] long companyId)
    {
        return Ok( await _service.GetByCompanyId(companyId));
    }
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var user = await _service.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var id = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateUserRequest request)
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