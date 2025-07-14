using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(long id);
    Task<IEnumerable<Invoice>> GetAllAsync();
}