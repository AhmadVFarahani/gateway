using Gateway.Application.AccessPolicies.Dtos;
using Gateway.Application.ApiKeys;
using Gateway.Application.Users;

namespace Gateway.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(long id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<IEnumerable<UserDto>> GetByCompanyId(long companyId);
    Task<long> CreateAsync(CreateUserRequest request);
    Task UpdateAsync(long id, UpdateUserRequest request);
    Task DeleteAsync(long id);
    Task<byte[]> ExportToExcel();

    #region AccessPolicies
    Task<IEnumerable<AccessPolicyDto>> GetAccessPoliciesAsync(long userId);
    Task<AccessPolicyDto> GetAccessPolicyByIdAsync(long userId, long accessPolicyId);
    Task<long> AddAccessPolicyToUserAsync(long userId, CreateAccessPolicyRequest request);

    Task DeleteAccessPolicyAsync(long userId, long accessPolicyId);
    //Task UpdateAccessPolicy(long companyId, long companyPlanId, updateAccessPolicyRequest request);
    #endregion AccessPolicies

    #region APIKeys
    Task<IEnumerable<ApiKeyDto>> GetApiKeysAsync(long userId);
    #endregion APIKeys
}
