using Gateway.Application.Interfaces;
using Gateway.Application.Invoice.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _service;

    public InvoiceController(IInvoiceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => 
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var role = await _service.GetByIdAsync(id);
        return role == null ? NotFound() : Ok(role);
    }
}
