using Gateway.Application.Base;
using Gateway.Domain.Enums;

namespace Gateway.Application.Invoice.Dtos;

public class InvoiceListDto:BaseDto
{
    public long CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public long ContractId { get; set; }
    public string ContractDescription { get; set; } = string.Empty;


    public DateTime PeriodFrom { get; set; }
    public DateTime PeriodTo { get; set; }

    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; }

}
