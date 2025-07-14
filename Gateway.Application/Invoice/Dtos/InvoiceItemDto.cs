using Gateway.Application.Base;

namespace Gateway.Application.Invoice.Dtos;

public class InvoiceItemDto : BaseDto
{
    public long InvoiceId { get; set; }

    public long? RouteId { get; set; } // Nullable if only overall item

    public int UsageCount { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }

    public string? TierDetails { get; set; } // Optional: JSON or description

}