using Account.Application.Common.Interfaces;
using Account.Domain.Common;
using Account.Domain.Common.DateTimes;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Persistence.Context;

public abstract class BaseDbContext : DbContext
{
    private readonly ICurrentUserProvider? _currentUserProvider;

    public BaseDbContext(DbContextOptions options, ICurrentUserProvider? currentUserProvider = null) : base(options)
    {
        _currentUserProvider = currentUserProvider;
    }

    private string NormalizeUserId()
    {
        var currentUserId = _currentUserProvider?.CurrentUserId;
        if (Guid.TryParse(currentUserId, out var guid)) return guid.ToString();
        return "system";
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var userId = NormalizeUserId();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreated(Clock.Now, userId);
                    break;
                case EntityState.Modified:
                    if (!entry.Property(nameof(BaseEntity.UpdatedAt)).IsModified &&
                        !entry.Property(nameof(BaseEntity.UpdatedBy)).IsModified)
                    {
                        entry.Entity.SetUpdated(Clock.Now, userId);
                    }
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.SetDeleted(Clock.Now, userId);
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
