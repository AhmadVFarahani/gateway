
using Gateway.Application.Interfaces;
using Gateway.Application.Routes.Dtos;
using Gateway.Application.Services.Dtos;
using Gateway.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json;

namespace Gateway.Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _service;

    public ServicesController(IServiceService service)
    {
        _service = service;
    }

    [HttpPost("import-route-from-json")]
    public async Task<IActionResult> ImportFromJson([FromBody] string address)
    {
        RouteDto route = new RouteDto();

        try
        {
            using var httpClient = new HttpClient();

            var swaggerUrl = address;

            var response = await httpClient.GetAsync(swaggerUrl);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

          

            if (root.TryGetProperty("paths", out JsonElement paths))
            {
                foreach (var path in paths.EnumerateObject())
                {
                    Console.WriteLine("Route: " + path.Name);
                }
            }


            return BadRequest("Invalid JSON structure.");
        }
        catch (Exception ex)
        {
            return BadRequest($"JSON parsing failed: {ex.Message}");
        }

        return Ok();
    }

        //    // مرحله 2: ساختن entity برای ذخیره در دیتابیس
        //    var route = new Route
        //    {
        //        Path = dto.Path,
        //        HttpMethod = dto.HttpMethod,
        //        TargetPath = dto.TargetPath,
        //        RequiresAuthentication = dto.RequiresAuthentication,
        //        IsActive = dto.IsActive,
        //        ServiceId = dto.ServiceId,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    route.RouteRequestFields = dto.RequestFields.Select(f => new RouteRequestField
        //    {
        //        FieldName = f.FieldName,
        //        Type = f.Type,
        //        Format = f.Format,
        //        IsRequired = f.IsRequired,
        //        Description = f.Description,
        //        ParentId = f.ParentId,
        //        CreatedAt = DateTime.UtcNow
        //    }).ToList();

        //    route.RouteResponseFields = dto.ResponseFields.Select(f => new RouteResponseField
        //    {
        //        Name = f.Name,
        //        Type = f.Type,
        //        IsRequired = f.IsRequired,
        //        Description = f.Description,
        //        ParentId = f.ParentId,
        //        CreatedAt = DateTime.UtcNow
        //    }).ToList();

        //    _context.Routes.Add(route);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetById), new { id = route.Id }, null);
        //}

        [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var service = await _service.GetByIdAsync(id);
        return service == null ? NotFound() : Ok(service);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceRequest request)
    {
        var id = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateServiceRequest request)
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
