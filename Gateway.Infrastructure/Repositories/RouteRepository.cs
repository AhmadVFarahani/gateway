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
        return await _context.Routes.Include(c=>c.Service).ToListAsync();
    }

    public async Task<IEnumerable<Route>> GetByServiceId(long serviceId)
    {
        return await _context.Routes.Include(c=>c.Service).Where(c=>c.ServiceId==serviceId).ToListAsync();
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

    public async Task<Route?> GetByIdWithFieldsAsync(long id)
    {
        return await _context.Routes
            .Include(c => c.ResponseFields)
            .Include(c => c.RequestFields)
            .FirstOrDefaultAsync(c => c.Id == id); 
    }
}
