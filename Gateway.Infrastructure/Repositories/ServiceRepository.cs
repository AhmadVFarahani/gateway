using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Repositories;
public class ServiceRepository : IServiceRepository
{
    private readonly GatewayDbContext _context;

    public ServiceRepository(GatewayDbContext context)
    {
        _context = context;
    }

    public async Task<Service?> GetByIdAsync(long id) =>
        await _context.Services.FindAsync(id);

    public async Task<IEnumerable<Service>> GetAllAsync() =>
        await _context.Services.ToListAsync();

    public async Task AddAsync(Service service)
    {
        _context.Services.Add(service);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Service service)
    {
        _context.Services.Update(service);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Service service)
    {
        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
    }
}