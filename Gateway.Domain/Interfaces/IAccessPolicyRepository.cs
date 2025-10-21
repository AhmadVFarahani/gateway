using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IAccessPolicyRepository
{
    Task<IEnumerable<AccessPolicy>> GetAllAsync();
}
