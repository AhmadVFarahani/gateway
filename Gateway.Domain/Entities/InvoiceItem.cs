namespace Gateway.Domain.Entities;

public class InvoiceItem : BaseEntity
{
    public long InvoiceId { get; set; }

    public Guid? RouteId { get; set; } // Nullable if only overall item

    public int UsageCount { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }

    public string? TierDetails { get; set; } // Optional: JSON or description

    public Invoice Invoice { get; set; } = default!;
}