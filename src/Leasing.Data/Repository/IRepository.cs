using Leasing.Domain.Models;

namespace Leasing.Data.Repository;

public interface IRepository<in TKey, TEntity>
    where TEntity : class, IEntity
    where TKey : IEquatable<TKey>
{
    Task<IReadOnlyCollection<TEntity>> GetAllAsync(bool asNoTracking = true);

    Task<TEntity> GetByIdAsync(TKey id);

    Task<TEntity> AddAsync(TEntity entity);
}