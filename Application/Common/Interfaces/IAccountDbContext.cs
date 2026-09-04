using Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Common.Interfaces;

public interface IAccountDbContext
{
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}