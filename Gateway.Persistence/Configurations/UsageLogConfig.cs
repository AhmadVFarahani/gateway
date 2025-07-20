using Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Persistence.Configurations;

public class UsageLogConfig : IEntityTypeConfiguration<UsageLog>
{
    public void Configure(EntityTypeBuilder<UsageLog> builder)
    {

        builder.ToTable("UsageLogs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CompanyId)
            .IsRequired();
        builder.Property(x => x.RouteId)
            .IsRequired();
        builder.Property(x => x.ContractId)
            .IsRequired();
        builder.Property(x => x.DurationMs)
            .IsRequired();
        builder.Property(x => x.IsBilled)
            .HasDefaultValue(false)
            .IsRequired();
        builder.Property(x => x.Timestamp)
           .IsRequired();
        builder.Property(x => x.ResponseStatusCode)
          .IsRequired();
    }
}
