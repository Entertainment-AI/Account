using Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Common.Interfaces;

public interface IAccountDbContext
{
    DbSet<User> Users { get; }
    DbSet<Plan> Plans { get; }
    DbSet<UserSubscription> UserSubscriptions { get; }
    DbSet<Order> Orders { get; }
    DbSet<Wallet> Wallets { get; }
    DbSet<WalletTransaction> WalletTransactions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}