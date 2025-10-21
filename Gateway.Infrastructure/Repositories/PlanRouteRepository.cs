using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Repositories;

public class PlanRouteRepository : IPlanRouteRepository
{
    private readonly GatewayDbContext _context;

    public PlanRouteRepository(GatewayDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<PlanRoute>> GetAllAsync()
    {
        return await _context.PlanRoutes.AsNoTracking().ToListAsync();
    }
}