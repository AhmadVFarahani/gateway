using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Repositories;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly GatewayDbContext _context;

    public ApiKeyRepository(GatewayDbContext context)
    {
        _context = context;
    }

    public async Task<ApiKey?> GetByKeyAsync(string key)
    {
        return await _context.ApiKeys.FirstOrDefaultAsync(x => x.Key == key);
    }

    public async Task<ApiKey?> GetByIdAsync(long id)
    {
        return await _context.ApiKeys.FindAsync(id);
    }

    public async Task<IEnumerable<ApiKey>> GetAllAsync()
    {
        return await _context.ApiKeys.ToListAsync();
    }

    public async Task AddAsync(ApiKey apiKey)
    {
        _context.ApiKeys.Add(apiKey);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ApiKey apiKey)
    {
        _context.ApiKeys.Remove(apiKey);
        await _context.SaveChangesAsync();
    }
}
