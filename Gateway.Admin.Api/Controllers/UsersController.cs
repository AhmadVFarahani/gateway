using Gateway.Application.AccessPolicies.Dtos;
using Gateway.Application.Interfaces;
using Gateway.Application.Users;
using Gateway.Domain.Entities;
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


    #region AccessPolicies
    [HttpGet("{userId}/accessPolicies")]
    public async Task<IActionResult> GetAccessPolicies(long userId) =>
        Ok(await _service.GetAccessPoliciesAsync(userId));

    [HttpGet("{userId}/accessPolicies/{accessPolicyId}")]
    public async Task<IActionResult> GetAccessPolicyById(long userId, long accessPolicyId) =>
       Ok(await _service.GetAccessPolicyByIdAsync(userId, accessPolicyId));


    [HttpPost("{userId}/accessPolicies")]
    public async Task<IActionResult> AddAccessPolicyToUser(long userId, [FromBody] CreateAccessPolicyRequest request)
    {
        var id = await _service.AddAccessPolicyToUserAsync(userId, request);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
    [HttpDelete("{userId}/accessPolicies/{accessPolicyId}")]
    public async Task<IActionResult> DeleteAccessPolicy(long userId, long accessPolicyId)
    {
        await _service.DeleteAccessPolicyAsync(userId, accessPolicyId);
        return NoContent();
    }
    //[HttpPut("{userId}/accessPolicies/{accessPolicyId}")]
    //public async Task<IActionResult> UpdateAccessPlan(long userId, long accessPolicyId, [FromBody] UpdateCompanyPlanRequest request)
    //{
    //    await _service.UpdateAccessPlan(userId, accessPolicyId, request);
    //    return NoContent();
    //}
    #endregion AccessPolicies

    #region APIKeys
    [HttpGet("{userId}/apiKeys")]
    public async Task<IActionResult> GetApiKeys(long userId) =>
        Ok(await _service.GetApiKeysAsync(userId));

    #endregion APIKeys

}