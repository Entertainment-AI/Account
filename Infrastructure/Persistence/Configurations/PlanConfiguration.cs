using Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("Id").ValueGeneratedNever();

        builder.Property(p => p.Name).HasColumnName("Name").HasMaxLength(100).IsRequired();
        builder.Property(p => p.Code).HasColumnName("Code").HasMaxLength(50).IsRequired();
        builder.HasIndex(p => p.Code).IsUnique();

        builder.Property(p => p.Price).HasColumnName("Price").HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.DurationMonths).HasColumnName("DurationMonths").IsRequired();
        builder.Property(p => p.Description).HasColumnName("Description").HasMaxLength(500);
        builder.Property(p => p.Features).HasColumnName("Features");
        builder.Property(p => p.IsActive).HasColumnName("IsActive").IsRequired();

        builder.Property(p => p.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(p => p.UpdatedAt).HasColumnName("UpdatedAt");
        builder.Property(p => p.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(100);
        builder.Property(p => p.UpdatedBy).HasColumnName("UpdatedBy").HasMaxLength(100);
        builder.Property(p => p.Deleted).HasColumnName("IsSoftDeleted").IsRequired();

        builder.Ignore(p => p.DeletedAt);
        builder.Ignore(p => p.DeletedBy);

        builder.HasQueryFilter(p => !p.Deleted);
    }
}
