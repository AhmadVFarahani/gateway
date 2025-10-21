using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Repositories;

public class AccessPolicyRepository : IAccessPolicyRepository
{
    private readonly GatewayDbContext _context;

    public AccessPolicyRepository(GatewayDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<AccessPolicy>> GetAllAsync()
    {
        return await _context.AccessPolicies.AsNoTracking().ToListAsync();
    }
}
