using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Domain.Views;
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

    public async Task<InvoiceView?> GetByIdAsync(long id)
    {
        var invoice =await _context.InvoiceViews.AsNoTracking().Where(x => x.Id == id).SingleAsync();
        invoice.Items = await _context.InvoiceItemViews.AsNoTracking().Where(c=>c.InvoiceId==id).ToListAsync(); 
        return invoice;
    }

    public async Task<IEnumerable<InvoiceView>> GetAllAsync() =>
        await _context.InvoiceViews.AsNoTracking().ToListAsync();
   
}