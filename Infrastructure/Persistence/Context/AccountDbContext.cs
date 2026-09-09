using Account.Application.Common.Interfaces;
using Account.Domain.Entities;
using Account.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Persistence.Context;

public class AccountDbContext : BaseDbContext
{
    public AccountDbContext(
        DbContextOptions<AccountDbContext> options,
        ICurrentUserProvider? currentUserProvider = null) 
        : base(options, currentUserProvider)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountDbContext).Assembly);
    }
}
