using Gateway.Application.Invoice.Dtos;

namespace Gateway.Application.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceDto?> GetByIdAsync(long id);
    Task<IEnumerable<InvoiceListDto>> GetAllAsync();
   
}