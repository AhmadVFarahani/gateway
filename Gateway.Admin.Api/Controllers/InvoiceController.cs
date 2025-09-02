using Gateway.Application.Interfaces;
using Gateway.Application.Invoice.Dtos;
using Gateway.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

    [HttpGet("export")]
    public async Task<IActionResult> ExportInvoices()
    {
        var fileBytes =await _service.ExportToExcel();

        return File(fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Invoices.xlsx");
    }
}
