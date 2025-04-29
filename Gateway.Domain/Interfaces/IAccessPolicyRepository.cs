using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IAccessPolicyRepository
{
    Task<AccessPolicy?> GetByIdAsync(long id);
    Task<IEnumerable<AccessPolicy>> GetAllAsync();
    Task AddAsync(AccessPolicy policy);
    Task DeleteAsync(AccessPolicy policy);
}