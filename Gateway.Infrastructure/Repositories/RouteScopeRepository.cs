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
        return await _context.RouteScopes.AsNoTracking().ToListAsync();
    }
}
