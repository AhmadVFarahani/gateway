using Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Persistence.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("Contracts");

        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.Description)
            .IsRequired()
            .HasDefaultValue("-")
            .HasMaxLength(100);

        builder.HasOne(cp => cp.Company)
            .WithMany(c=>c.CompanyPlans)
            .HasForeignKey(cp => cp.CompanyId);

        builder.HasOne(cp => cp.Plan)
            .WithMany(p => p.CompanyPlans)
            .HasForeignKey(cp => cp.PlanId);

        builder.Property(cp => cp.IsActive).IsRequired();
        builder.Property(cp => cp.AutoRenew).IsRequired();
        builder.Property(cp => cp.StartDate).IsRequired();

        builder.Property(cp => cp.CreatedAt).IsRequired();
        builder.Property(cp => cp.UpdatedAt).IsRequired(false);
    }
}
