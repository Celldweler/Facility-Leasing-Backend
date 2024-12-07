using Leasing.Api.Domain;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.EntityFrameworkCore;

namespace Leasing.Api.Data.Repository;

public abstract class EntityRepositoryBase<TKey, TEntity> : IRepository<TKey, TEntity>
    where TEntity : class, IEntity
    where TKey : IEquatable<TKey>
{
    private readonly LeasingDataContext _context;
    protected readonly DbSet<TEntity> _dbEntities;

    protected EntityRepositoryBase(LeasingDataContext context)
    {
        _context = context;
        _dbEntities = _context.Set<TEntity>();
    }
    
    public virtual async Task<IReadOnlyCollection<TEntity>> GetAllAsync(bool asNoTracking = true)
    {
        var query = _dbEntities.AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }
        
        return (await query.ToListAsync()).AsReadOnly();
    }

    public abstract Task<TEntity> GetByIdAsync(TKey id);

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await _dbEntities.AddAsync(entity);
        await _context.SaveChangesAsync();
        
        return entity;
    }
}