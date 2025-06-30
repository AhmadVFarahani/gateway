using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Repositories;
public class RouteRepository : IRouteRepository
{
    private readonly GatewayDbContext _context;

    public RouteRepository(GatewayDbContext context)
    {
        _context = context;
    }

    public async Task<Route?> GetByIdAsync(long id)
    {
        return await _context.Routes.FindAsync(id);
    }

    public async Task<IEnumerable<Route>> GetAllAsync()
    {
        return await _context.Routes.ToListAsync();
    }

    public async Task<IEnumerable<Route>> GetByServiceId(long serviceId)
    {
        return await _context.Routes.Where(c=>c.ServiceId==serviceId).ToListAsync();
    }

    public async Task AddAsync(Route route)
    {
        _context.Routes.Add(route);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Route route)
    {
        _context.Routes.Update(route);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Route route)
    {
        _context.Routes.Remove(route);
        await _context.SaveChangesAsync();
    }
    public async Task<Route?> GetByPathAsync(string path)
    {
        return await _context.Routes
            .FirstOrDefaultAsync(r => r.Path.ToLower() == path.ToLower());
    }
}
