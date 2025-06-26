using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly GatewayDbContext _context;

    public RoleRepository(GatewayDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(long id) =>
        await _context.Roles.FindAsync(id);

    public async Task<IEnumerable<Role>> GetAllAsync() =>
        await _context.Roles.ToListAsync();

    public async Task AddAsync(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Role role)
    {
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Role role)
    {
        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
    }
}