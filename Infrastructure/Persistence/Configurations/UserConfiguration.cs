using Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("Id").ValueGeneratedNever();

        builder.Property(u => u.Email).HasColumnName("Email").HasMaxLength(150).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash).HasColumnName("PasswordHash").HasMaxLength(255).IsRequired();
        builder.Property(u => u.UserName).HasColumnName("UserName").HasMaxLength(100);
        builder.Property(u => u.DisplayName).HasColumnName("DisplayName").HasMaxLength(100);
        builder.Property(u => u.AvatarUrl).HasColumnName("AvatarUrl").HasMaxLength(500);

        builder.Property(u => u.Role).HasColumnName("Role").HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(u => u.IsEmailVerified).HasColumnName("IsEmailVerified").IsRequired();
        builder.Property(u => u.LastUserNameChangedAt).HasColumnName("LastUserNameChangedAt");

        builder.Property(u => u.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(u => u.UpdatedAt).HasColumnName("UpdatedAt");
        builder.Property(u => u.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(100);
        builder.Property(u => u.UpdatedBy).HasColumnName("UpdatedBy").HasMaxLength(100);
        builder.Property(u => u.Deleted).HasColumnName("IsSoftDeleted").IsRequired();

        builder.Ignore(u => u.DeletedAt);
        builder.Ignore(u => u.DeletedBy);

        builder.HasQueryFilter(u => !u.Deleted);
    }
}
