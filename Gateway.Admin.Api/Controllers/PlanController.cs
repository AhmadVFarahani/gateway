using Gateway.Application.Interfaces;
using Gateway.Application.Plan.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public  class PlanController : ControllerBase
{
    private readonly IPlanService _service;

    public PlanController(IPlanService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var plan = await _service.GetByIdAsync(id);
        return plan == null ? NotFound() : Ok(plan);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanRequest request)
    {
        var id = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, new {id});
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdatePlanRequest request)
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
