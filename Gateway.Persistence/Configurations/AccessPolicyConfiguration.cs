using Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gateway.Persistence.Configurations;

public class AccessPolicyConfiguration : IEntityTypeConfiguration<AccessPolicy>
{
    public void Configure(EntityTypeBuilder<AccessPolicy> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ApiKey)
            .WithMany()
            .HasForeignKey(x => x.ApiKeyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Scope)
            .WithMany()
            .HasForeignKey(x => x.ScopeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}