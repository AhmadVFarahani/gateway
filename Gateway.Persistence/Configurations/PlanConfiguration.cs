using Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Persistence.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(p => p.MonthlyPrice)
              .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.RequestPrice)
              .IsRequired()
           .HasColumnType("decimal(18,2)");

        builder.Property(p => p.PricingType)
            
              .IsRequired();


        builder.Property(p => p.IsUnlimited)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired(false);
    }
}
