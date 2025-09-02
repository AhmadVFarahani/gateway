using Gateway.Domain.Entities;
using Gateway.Domain.Views;

namespace Gateway.Domain.Interfaces;

public interface IInvoiceRepository
{
    Task<InvoiceView?> GetByIdAsync(long id);
    Task<IEnumerable<InvoiceView>> GetAllAsync();
}