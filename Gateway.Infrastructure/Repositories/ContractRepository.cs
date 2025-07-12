using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Repositories;

public class ContractRepository : IContractRepository
{
    private readonly GatewayDbContext _context;

    public ContractRepository(GatewayDbContext context)
    {
        _context = context;
    }

    public async Task<Contract?> GetByIdAsync(long id) =>
        await _context.Contracts.FindAsync(id);

    public async Task<IEnumerable<Contract>> GetAllAsync() =>
        await _context.Contracts.ToListAsync();

    public async Task AddAsync(Contract role)
    {
        _context.Contracts.Add(role);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Contract role)
    {
        _context.Contracts.Update(role);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Contract role)
    {
        _context.Contracts.Remove(role);
        await _context.SaveChangesAsync();
    }
}