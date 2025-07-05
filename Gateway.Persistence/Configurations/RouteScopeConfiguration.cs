using Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Persistence.Configurations;

public class RouteScopeConfiguration : IEntityTypeConfiguration<RouteScope>
{
    public void Configure(EntityTypeBuilder<RouteScope> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Route)
            .WithMany()
            .HasForeignKey(x => x.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Scope)
            .WithMany(c => c.RouteScopes)
            .HasForeignKey(x => x.ScopeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}