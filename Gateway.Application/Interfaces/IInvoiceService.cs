using Gateway.Application.Invoice.Dtos;
using Gateway.Domain.Views;

namespace Gateway.Application.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceView?> GetByIdAsync(long id);
    Task<IEnumerable<InvoiceListDto>> GetAllAsync();

    Task<byte[]> ExportToExcel();


}