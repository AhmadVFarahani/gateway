
using Gateway.Application.Company.Dtos;
using Gateway.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _service;

    public CompanyController(ICompanyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var company = await _service.GetByIdAsync(id);
        return company == null ? NotFound() : Ok(company);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request)
    {
        var id = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateCompanyRequest request)
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


    #region Plan
    [HttpGet("{companyId}/plans")]
    public async Task<IActionResult> GetCompanyPlans(long companyId) =>
        Ok(await _service.GetCompanyPlanssAsync(companyId));

    [HttpGet("{companyId}/plans/{companyPlanId}")]
    public async Task<IActionResult> GetCompanyPlanById(long companyId, long companyPlanId) =>
       Ok(await _service.GetCompanyPlanByIdsAsync(companyId, companyPlanId));


    [HttpPost("{companyId}/plans")]
    public async Task<IActionResult> AddPlanToCompany(long companyId, [FromBody] CreateCompanyPlanRequest request)
    {
        var id = await _service.AddPlanToCompanysAsync(companyId, request);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{companyId}/plans/{companyPlanId}")]
    public async Task<IActionResult> UpdateCompanyPlan(long companyId, long companyPlanId, [FromBody] UpdateCompanyPlanRequest request)
    {
        await _service.UpdateCompanyPlansAsync(companyId, companyPlanId, request);
        return NoContent();
    }
    #endregion Plan

    #region RoutePricing
    [HttpGet("{companyId}/RoutePricing")]
    public async Task<IActionResult> GetCompanyRoutingPrices(long companyId) =>
        Ok(await _service.GetCompanyRoutePricingssAsync(companyId));

    [HttpGet("{companyId}/RoutePricing/{routingPriceId}")]
    public async Task<IActionResult> GetCompanyRoutingPriceById(long companyId, long routingPriceId) =>
       Ok(await _service.GetCompanyRoutePricingByIdsAsync(companyId, routingPriceId));


    [HttpPost("{companyId}/RoutePricing")]
    public async Task<IActionResult> AddRoutingPriceToCompany(long companyId, [FromBody] CreateCompanyRoutePricingRequest request)
    {
        var id = await _service.AddRoutePricingToCompanysAsync(companyId, request);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{companyId}/RoutePricing/{companyPlanId}")]
    public async Task<IActionResult> UpdateRoutingPrice(long companyId, long routingPriceId, [FromBody] UpdateCompanyRoutePricingRequest request)
    {
        await _service.UpdateCompanyRoutePricingAsync(companyId, routingPriceId, request);
        return NoContent();
    }

    [HttpDelete("{scopeId}/RoutePricing/{routingPriceId}")]
    public async Task<IActionResult> DeleteRouteScope(long companyId, long routingPriceId)
    {
        await _service.DeleteRoutePricingAsync(companyId, routingPriceId);
        return NoContent();
    }
    #endregion RoutePricing
}