using Gateway.Application.Interfaces;
using Gateway.Application.Plan.Dtos;
using Gateway.Application.RouteScopes.Dtos;
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

    #region Route
    [HttpGet("{planId}/routes")]
    public async Task<IActionResult> GetPlanRoutes(long planId) =>
        Ok(await _service.GetPlanRouteAsync(planId));

    [HttpGet("{planId}/routes/{planRouteId}")]
    public async Task<IActionResult> GetPlanRouteById(long planId, long planRouteId) =>
       Ok(await _service.GetPlanRouteByIdAsync(planId, planRouteId));


    [HttpPost("{planId}/routes")]
    public async Task<IActionResult> AddRouteToPlan(long planId, [FromBody] CreatePlanRouteRequest request)
    {
        var id = await _service.AddRouteToPlanAsync(planId, request);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{userId}/routes/{planRouteId}")]
    public async Task<IActionResult> UpdatePlanRoute(long planId, long planRouteId, [FromBody] UpdatePlanRouteRequest request)
    {
        await _service.UpdatePlanRouteAsync(planId, planRouteId, request);
        return NoContent();
    }


    [HttpDelete("{planId}/routes/{planRouteId}")]
    public async Task<IActionResult> DeleteRouteScope(long planId, long planRouteId)
    {
        await _service.DeleteRoutePlanAsync(planId, planRouteId);
        return NoContent();
    }

    #endregion Route
}
