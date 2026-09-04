using Account.Domain.Common;

namespace Account.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
