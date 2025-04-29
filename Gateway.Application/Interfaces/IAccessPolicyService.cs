using Gateway.Application.AccessPolicies.Dtos;

namespace Gateway.Application.Interfaces;

public interface IAccessPolicyService
{
    Task<IEnumerable<AccessPolicyDto>> GetAllAsync();
    Task<long> CreateAsync(CreateAccessPolicyRequest request);
    Task DeleteAsync(long id);
}