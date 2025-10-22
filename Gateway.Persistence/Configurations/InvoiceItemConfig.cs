using Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Persistence.Configurations;

public class InvoiceItemConfig : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RouteId).IsRequired(false);

        builder.Property(x => x.TierDetails)
               .HasColumnType("nvarchar(200)");

        builder.Property(x => x.UsageCount)
               .IsRequired();

        builder.Property(x => x.UnitPrice)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(x => x.SubTotal)
               .IsRequired()
               .HasPrecision(18, 2);
    }
}