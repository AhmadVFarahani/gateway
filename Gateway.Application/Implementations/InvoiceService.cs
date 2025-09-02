using AutoMapper;
using ClosedXML.Excel;
using Gateway.Application.Interfaces;
using Gateway.Application.Invoice.Dtos;
using Gateway.Domain.Interfaces;
using Gateway.Domain.Views;

namespace Gateway.Application.Implementations;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repository;
    private readonly IMapper _mapper;

    public InvoiceService(IInvoiceRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<InvoiceView?> GetByIdAsync(long id)
    {
        return await _repository.GetByIdAsync(id);

    }
    public async Task< byte[]> ExportToExcel()
    {
        var invoices =(await _repository.GetAllAsync()).ToList();
      
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Invoices");

        // Header
        worksheet.Cell(1, 1).Value = "CompanyId";
        worksheet.Cell(1, 2).Value = "CompanyName";
        worksheet.Cell(1, 3).Value = "ContractId";
        worksheet.Cell(1, 4).Value = "ContractDescription";
        worksheet.Cell(1, 5).Value = "PeriodFrom";
        worksheet.Cell(1, 6).Value = "PeriodTo";
        worksheet.Cell(1, 7).Value = "TotalAmount";
        worksheet.Cell(1, 8).Value = "Status";

        // Data
        for (int i = 0; i < invoices.Count; i++)
        {
            var row = i + 2; 
            var inv = invoices[i];

            worksheet.Cell(row, 1).Value = inv.CompanyId;
            worksheet.Cell(row, 2).Value = inv.CompanyName;
            worksheet.Cell(row, 3).Value = inv.ContractId;
            worksheet.Cell(row, 4).Value = inv.ContractDescription;
            worksheet.Cell(row, 5).Value = inv.PeriodFrom;
            worksheet.Cell(row, 6).Value = inv.PeriodTo;
            worksheet.Cell(row, 7).Value = inv.TotalAmount;
            worksheet.Cell(row, 8).Value = inv.Status.ToString();
        }

        // Auto-size columns
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray(); 
    }

    public async Task<IEnumerable<InvoiceListDto>> GetAllAsync()
    {
        var ivoices = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<InvoiceListDto>>(ivoices);
    }

}