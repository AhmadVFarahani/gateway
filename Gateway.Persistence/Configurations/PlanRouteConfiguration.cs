using Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Persistence.Configurations;

public class PlanRouteConfiguration : IEntityTypeConfiguration<PlanRoute>
{
    public void Configure(EntityTypeBuilder<PlanRoute> builder)
    {
        builder.ToTable("PlanRoutes");

        builder.HasKey(pr => pr.Id);

        builder.HasOne(pr => pr.Plan)
            .WithMany(p => p.PlanRoutes)
            .HasForeignKey(pr => pr.PlanId);

        builder.HasOne(pr => pr.Route)
            .WithMany(c=>c.PlanRoutes)
            .HasForeignKey(pr => pr.RouteId);

        builder.Property(pr => pr.FlatPricePerCall)
            .HasColumnType("decimal(18,2)");

        builder.Property(pr => pr.TieredPricingJson)
            .HasColumnType("nvarchar(200)");

        builder.Property(pr => pr.IsFree).IsRequired();
        builder.Property(pr => pr.CreatedAt).IsRequired();
        builder.Property(pr => pr.UpdatedAt).IsRequired(false);
    }
}
