using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id);
    Task<User?> GetWithAccessPolicyAsync(long id);
    Task<User?>  GetWithApiKeyAsync(long id);
    Task<AccessPolicy?> GetAccessPolicyByIdAsync(long userId, long accessPlanId);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetByCompanyId(long companyId);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}
