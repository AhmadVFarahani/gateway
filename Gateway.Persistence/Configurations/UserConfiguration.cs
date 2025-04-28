using Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.UserType)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}