using Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Persistence.Configurations;

public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Path)
            .IsRequired()
            .HasMaxLength(70);

        builder.Property(x => x.HttpMethod)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(x => x.TargetPath)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.RequiresAuthentication)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}
