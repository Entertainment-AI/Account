using Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasColumnName("Id").ValueGeneratedNever();

        builder.Property(o => o.OrderCode).HasColumnName("OrderCode").HasMaxLength(50).IsRequired();
        builder.HasIndex(o => o.OrderCode).IsUnique();

        builder.Property(o => o.UserId).HasColumnName("UserId").IsRequired();
        builder.HasIndex(o => o.UserId);

        builder.Property(o => o.PlanId).HasColumnName("PlanId");
        builder.Property(o => o.Type).HasColumnName("Type").HasConversion<string>().HasMaxLength(30).IsRequired();

        builder.Property(o => o.Amount).HasColumnName("Amount").HasPrecision(18, 2).IsRequired();
        builder.Property(o => o.DiscountAmount).HasColumnName("DiscountAmount").HasPrecision(18, 2).IsRequired();
        builder.Property(o => o.FinalAmount).HasColumnName("FinalAmount").HasPrecision(18, 2).IsRequired();

        builder.Property(o => o.Status).HasColumnName("Status").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(o => o.PaymentMethod).HasColumnName("PaymentMethod").HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(o => o.PaidAt).HasColumnName("PaidAt");
        builder.Property(o => o.Metadata).HasColumnName("Metadata").HasColumnType("jsonb");

        builder.HasOne<User>().WithMany().HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Plan>().WithMany().HasForeignKey(o => o.PlanId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(o => o.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(o => o.UpdatedAt).HasColumnName("UpdatedAt");
        builder.Property(o => o.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(100);
        builder.Property(o => o.UpdatedBy).HasColumnName("UpdatedBy").HasMaxLength(100);
        builder.Property(o => o.Deleted).HasColumnName("IsSoftDeleted").IsRequired();

        builder.Ignore(o => o.DeletedAt);
        builder.Ignore(o => o.DeletedBy);

        builder.HasQueryFilter(o => !o.Deleted);
    }
}
