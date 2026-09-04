using Account.Application.Common.Interfaces;
using Account.Domain.Entities;
using Account.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Persistence.Context;

public class AccountDbContext : BaseDbContext, IAccountDbContext
{
    public AccountDbContext(
        DbContextOptions<AccountDbContext> options,
        ICurrentUserProvider? currentUserProvider = null) 
        : base(options, currentUserProvider)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
