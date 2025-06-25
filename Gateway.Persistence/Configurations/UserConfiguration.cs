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

        builder.HasOne(x => x.Company)
           .WithMany(c=>c.Users)
           .HasForeignKey(x => x.CompanyId)
           .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Role)
          .WithMany(c=>c.Users)
          .HasForeignKey(x => x.RoleId)
          .OnDelete(DeleteBehavior.Cascade);
    }
}
