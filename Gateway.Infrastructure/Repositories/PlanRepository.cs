using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly GatewayDbContext _context;

    public PlanRepository(GatewayDbContext context)
    {
        _context = context;
    }

    public async Task<Plan?> GetByIdAsync(long id) =>
        await _context.Plans.Include(c => c.PlanRoutes).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Plan>> GetAllAsync() =>
        await _context.Plans.ToListAsync();

    public async Task AddAsync(Plan plan)
    {
        _context.Plans.Add(plan);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Plan plan)
    {
        _context.Plans.Update(plan);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Plan plan)
    {
        _context.Plans.Remove(plan);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PlanRoute>> GetPlanRouteByPlanId(long planId)
    {
        return await _context.PlanRoutes
                      .Where(pr => pr.PlanId == planId)
                      .ToListAsync();
    }

    public async Task<IEnumerable<PlanRoute>> GetPlanRouteByRouteId(long routeId)
    {
        return await _context.PlanRoutes
                       .Where(pr => pr.RouteId == routeId)
                       .ToListAsync();
    }

    Task<PlanRoute?> IPlanRepository.GetPlanRouteByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

}