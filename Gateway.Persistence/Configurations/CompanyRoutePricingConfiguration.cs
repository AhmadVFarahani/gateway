using Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Persistence.Configurations;

public class CompanyRoutePricingConfiguration : IEntityTypeConfiguration<CompanyRoutePricing>
{
    public void Configure(EntityTypeBuilder<CompanyRoutePricing> builder)
    {
        builder.ToTable("CompanyRoutePricing");

        builder.HasKey(crp => crp.Id);

        builder.HasOne(crp => crp.Company)
            .WithMany()
            .HasForeignKey(crp => crp.CompanyId);

        builder.HasOne(crp => crp.Route)
            .WithMany()
            .HasForeignKey(crp => crp.RouteId);

        builder.Property(crp => crp.BillingType)
            .HasConversion<string>()   // Stored as string
            .IsRequired();

        builder.Property(crp => crp.PricePerCall)
            .HasColumnType("decimal(18,2)");

        builder.Property(crp => crp.MonthlySubscriptionPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(crp => crp.TieredJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(crp => crp.MaxFreeCallsPerMonth);

        builder.Property(crp => crp.IsActive).IsRequired();
        builder.Property(crp => crp.CreatedAt).IsRequired();
        builder.Property(crp => crp.UpdatedAt).IsRequired(false);


        builder.HasOne(cp => cp.Company)
            .WithMany(c => c.CompanyRoutePricings)
            .HasForeignKey(cp => cp.CompanyId);
    }
}
