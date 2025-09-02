using Gateway.Application.Interfaces;
using Gateway.Application.Routes.Dtos;
using Gateway.Application.RouteScopes.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Gateway.Admin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteService _service;

        public RoutesController(IRouteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("byService")]
        public async Task<IActionResult> GetByServiceId([FromQuery] long serviceId)
        {
            return Ok(await _service.GetByServiceId(serviceId));
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var route = await _service.GetByIdAsync(id);
            return route == null ? NotFound() : Ok(route);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRouteRequest request)
        {
            var id = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateRouteRequest request)
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

        [HttpGet("export")]
        public async Task<IActionResult> ExportInvoices()
        {
            var fileBytes = await _service.ExportToExcel();

            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Invoices.xlsx");
        }

        #region Fields
        [HttpPost("{routeId:long}/SaveResponseFields")]
        public async Task<IActionResult> SaveResponseFields(long routeId, List<RouteResponseFieldDto> fields)
        {
            await _service.SaveResponseFieldsAsync(routeId, fields);
            return NoContent();
        }
        [HttpGet("{routeId:long}/RequestFields")]
        public async Task<IActionResult> GetRequestFields(long routeId)
        {
            var tree = await _service.GetRequestSchemaAsync(routeId);
            if (tree == null || tree.Count == 0) return NotFound(new { message = "No request schema found." });
            return Ok(tree);
        }


        [HttpPost("{routeId:long}/SaveRequestFields")]
        public async Task<IActionResult> SaveRequestFields(long routeId, List<RouteRequestFieldDto> fields)
        {
            await _service.SaveRequestFieldsAsync(routeId, fields);
            return NoContent();
        }
        [HttpGet("{routeId:long}/ResponseFields")]
        public async Task<IActionResult> GetResponseFields(long routeId)
        {
            var tree = await _service.GetResponseSchemaAsync(routeId);
            if (tree == null || tree.Count == 0) return NotFound(new { message = "No response schema found." });
            return Ok(tree);
        }
        #endregion Fields
    }
}