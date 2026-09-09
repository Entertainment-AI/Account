using Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations;

public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.ToTable("WalletTransactions");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("Id").ValueGeneratedNever();

        builder.Property(t => t.TransactionCode).HasColumnName("TransactionCode").HasMaxLength(50).IsRequired();
        builder.HasIndex(t => t.TransactionCode).IsUnique();

        builder.Property(t => t.WalletId).HasColumnName("WalletId").IsRequired();
        builder.HasIndex(t => t.WalletId);

        builder.Property(t => t.Amount).HasColumnName("Amount").HasPrecision(18, 2).IsRequired();
        builder.Property(t => t.BalanceBefore).HasColumnName("BalanceBefore").HasPrecision(18, 2).IsRequired();
        builder.Property(t => t.BalanceAfter).HasColumnName("BalanceAfter").HasPrecision(18, 2).IsRequired();

        builder.Property(t => t.Type).HasColumnName("Type").HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(t => t.OrderId).HasColumnName("OrderId");
        builder.Property(t => t.Description).HasColumnName("Description").HasMaxLength(255).IsRequired();

        builder.HasOne<Order>().WithMany().HasForeignKey(t => t.OrderId).OnDelete(DeleteBehavior.SetNull);

        builder.Property(t => t.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(t => t.UpdatedAt).HasColumnName("UpdatedAt");
        builder.Property(t => t.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(100);
        builder.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy").HasMaxLength(100);
        builder.Property(t => t.Deleted).HasColumnName("IsSoftDeleted").IsRequired();

        builder.Ignore(t => t.DeletedAt);
        builder.Ignore(t => t.DeletedBy);

        builder.HasQueryFilter(t => !t.Deleted);
    }
}
