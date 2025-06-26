
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
        return CreatedAtAction(nameof(GetById), new { id }, new {id});
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
}
