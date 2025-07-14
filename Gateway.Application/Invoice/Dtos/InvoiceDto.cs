using Gateway.Application.Base;
using Gateway.Domain.Enums;

namespace Gateway.Application.Invoice.Dtos;

public class InvoiceDto : BaseDto
{
    public long CompanyId { get; set; }
    public long ContractId { get; set; }

    public DateTime PeriodFrom { get; set; }
    public DateTime PeriodTo { get; set; }

    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public ICollection<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
}
