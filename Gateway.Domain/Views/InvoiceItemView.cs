using Gateway.Domain.Entities;

namespace Gateway.Domain.Views;

public class InvoiceItemView : BaseEntity
{
    public long InvoiceId { get; set; }

    public long? RouteId { get; set; } // Nullable if only overall item
    public string RouthPath { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }

    public string? TierDetails { get; set; } // Optional: JSON or description
}