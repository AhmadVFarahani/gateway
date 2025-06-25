using Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Persistence.Configurations;

public class RouteResponseFieldConfiguration : IEntityTypeConfiguration<RouteResponseField>
{
    public void Configure(EntityTypeBuilder<RouteResponseField> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Type).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);

        builder.HasOne(x => x.Route)
               .WithMany()
               .HasForeignKey(x => x.RouteId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Parent)
               .WithMany(x => x.Children)
               .HasForeignKey(x => x.ParentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}