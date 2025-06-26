using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly GatewayDbContext _context;

    public CompanyRepository(GatewayDbContext context)
    {
        _context = context;
    }

    public async Task<Company?> GetByIdAsync(long id) =>
        await _context.Companies.FindAsync(id);

    public async Task<IEnumerable<Company>> GetAllAsync() =>
        await _context.Companies.ToListAsync();

    public async Task AddAsync(Company company)
    {
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Company company)
    {
        _context.Companies.Update(company);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Company company)
    {
        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();
    }
}