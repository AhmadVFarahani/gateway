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

    public async Task<AccessPolicy?> GetByIdAsync(long id)
    {
        return await _context.AccessPolicies.FindAsync(id);
    }

    public async Task<IEnumerable<AccessPolicy>> GetAllAsync()
    {
        return await _context.AccessPolicies.ToListAsync();
    }

    public async Task AddAsync(AccessPolicy policy)
    {
        _context.AccessPolicies.Add(policy);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(AccessPolicy policy)
    {
        _context.AccessPolicies.Remove(policy);
        await _context.SaveChangesAsync();
    }
}
