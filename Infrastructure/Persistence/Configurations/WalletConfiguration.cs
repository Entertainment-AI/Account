using Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets", t =>
        {
            t.HasCheckConstraint("CK_Wallets_Balance_NonNegative", "\"Balance\" >= 0");
        });

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id).HasColumnName("Id").ValueGeneratedNever();

        builder.Property(w => w.UserId).HasColumnName("UserId").IsRequired();
        builder.HasIndex(w => w.UserId).IsUnique();

        builder.Property(w => w.Balance).HasColumnName("Balance").HasPrecision(18, 2).IsRequired();
        builder.Property(w => w.Currency).HasColumnName("Currency").HasMaxLength(10).IsRequired();
        builder.Property(w => w.Status).HasColumnName("Status").HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasOne<User>().WithMany().HasForeignKey(w => w.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(w => w.Transactions).WithOne().HasForeignKey(nameof(WalletTransaction.WalletId)).OnDelete(DeleteBehavior.Cascade);

        builder.Property(w => w.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(w => w.UpdatedAt).HasColumnName("UpdatedAt");
        builder.Property(w => w.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(100);
        builder.Property(w => w.UpdatedBy).HasColumnName("UpdatedBy").HasMaxLength(100);
        builder.Property(w => w.Deleted).HasColumnName("IsSoftDeleted").IsRequired();

        builder.Ignore(w => w.DeletedAt);
        builder.Ignore(w => w.DeletedBy);

        builder.HasQueryFilter(w => !w.Deleted);
    }
}
