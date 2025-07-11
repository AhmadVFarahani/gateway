using Gateway.Domain.Enums;

namespace Gateway.Domain.Entities;

public class Invoice : BaseEntity
{

    public long CompanyId { get; set; }
    public long ContractId { get; set; }

    public DateTime PeriodFrom { get; set; }
    public DateTime PeriodTo { get; set; }

    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; } 


    public ICollection<InvoiceItem> Items { get; set; }=new List<InvoiceItem>();
}
