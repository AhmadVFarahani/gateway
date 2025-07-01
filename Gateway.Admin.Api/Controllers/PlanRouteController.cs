using Azure.Core;
using Gateway.Application.Interfaces;
using Gateway.Application.Plan.Dtos;
using Gateway.Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Admin.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public  class PlanRouteController : ControllerBase
{
    private readonly IPlanService _service;

    public PlanRouteController(IPlanService service)
    {
        _service = service;
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetPlanRouteById(long id)
    {
        var planRoute = await _service.GetPlanRouteByIdAsync(id);
        return planRoute == null ? NotFound() : Ok(planRoute);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanRouteRequest request)
    {
        var id = await _service.CreatePlanRouteAsync(request);
        return CreatedAtAction(nameof(GetPlanRouteById), new { id }, new { id });
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdatePlanRoute(long id, [FromBody] UpdatePlanRouteRequest request)
    {
        await _service.UpdatePlanRouteAsync(id, request);
        return NoContent();
    }

    [HttpGet("byRoute")]
    public async Task<IActionResult> GetPlanRouteByRouteId(long RouteId) =>
        Ok(await _service.GetPlanRouteByRouteId(RouteId));


    [HttpGet("byPlan")]
    public async Task<IActionResult> GetPlanRouteByPlanId(long PlanId)=>
       Ok(await _service.GetPlanRouteByPlanId(PlanId));
       
}
