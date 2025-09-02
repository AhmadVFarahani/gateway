using Gateway.Domain.Entities;
using Gateway.Domain.Enums;

namespace Gateway.Domain.Views;

public class InvoiceView:BaseEntity
{
    public long CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;

    public long ContractId { get; set; }
    public string ContractDescription { get; set; } = string.Empty;


    public DateTime PeriodFrom { get; set; }
    public DateTime PeriodTo { get; set; }

    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; }

    public ICollection<InvoiceItemView> Items { get; set; } = new List<InvoiceItemView>();
}
