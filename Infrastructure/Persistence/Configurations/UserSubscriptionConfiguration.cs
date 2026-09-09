using Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("UserSubscriptions");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("Id").ValueGeneratedNever();

        builder.Property(s => s.UserId).HasColumnName("UserId").IsRequired();
        builder.HasIndex(s => s.UserId);

        builder.Property(s => s.PlanId).HasColumnName("PlanId").IsRequired();
        builder.HasIndex(s => s.PlanId);

        builder.Property(s => s.OrderId).HasColumnName("OrderId");

        builder.Property(s => s.StartDate).HasColumnName("StartDate").IsRequired();
        builder.Property(s => s.EndDate).HasColumnName("EndDate").IsRequired();
        builder.Property(s => s.Status).HasColumnName("Status").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(s => s.AutoRenew).HasColumnName("AutoRenew").IsRequired();

        builder.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.Plan).WithMany().HasForeignKey(s => s.PlanId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(s => s.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(s => s.UpdatedAt).HasColumnName("UpdatedAt");
        builder.Property(s => s.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(100);
        builder.Property(s => s.UpdatedBy).HasColumnName("UpdatedBy").HasMaxLength(100);
        builder.Property(s => s.Deleted).HasColumnName("IsSoftDeleted").IsRequired();

        builder.Ignore(s => s.DeletedAt);
        builder.Ignore(s => s.DeletedBy);

        builder.HasQueryFilter(s => !s.Deleted);
    }
}
