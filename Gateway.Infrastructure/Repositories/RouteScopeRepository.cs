using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Repositories;

public class RouteScopeRepository : IRouteScopeRepository
{
    private readonly GatewayDbContext _context;

    public RouteScopeRepository(GatewayDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RouteScope>> GetAllAsync()
    {
        return await _context.RouteScopes.ToListAsync();
    }

    public async Task AddAsync(RouteScope routeScope)
    {
        _context.RouteScopes.Add(routeScope);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(RouteScope routeScope)
    {
        _context.RouteScopes.Remove(routeScope);
        await _context.SaveChangesAsync();
    }

    public async Task<RouteScope?> GetByIdAsync(long id)
    {
        return await _context.RouteScopes.FindAsync(id);
    }
    public async Task<IEnumerable<RouteScope>> GetByRouteIdAsync(long routeId)
    {
        return await _context.RouteScopes
            .Where(rs => rs.RouteId == routeId)
            .ToListAsync();
    }
}
