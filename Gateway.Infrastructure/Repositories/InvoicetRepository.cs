using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly GatewayDbContext _context;

    public InvoiceRepository(GatewayDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(long id) =>
        await _context.Invoices.Include(c=>c.Items).FirstOrDefaultAsync (c=>c.Id==id);

    public async Task<IEnumerable<Invoice>> GetAllAsync() =>
        await _context.Invoices.ToListAsync();
   
}