using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Repositories;

public class ScopeRepository : IScopeRepository
{
    private readonly GatewayDbContext _context;

    public ScopeRepository(GatewayDbContext context)
    {
        _context = context;
    }

    public async Task<Scope?> GetByIdAsync(long id)
    {
        return await _context.Scopes.FindAsync(id);
    }

    public async Task<IEnumerable<Scope>> GetAllAsync()
    {
        return await _context.Scopes.ToListAsync();
    }

    public async Task AddAsync(Scope scope)
    {
        _context.Scopes.Add(scope);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Scope scope)
    {
        _context.Scopes.Update(scope);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Scope scope)
    {
        _context.Scopes.Remove(scope);
        await _context.SaveChangesAsync();
    }
}