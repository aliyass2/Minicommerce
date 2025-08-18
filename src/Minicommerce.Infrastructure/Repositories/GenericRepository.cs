using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Minicommerce.Domain.Common;
using Minicommerce.Domain.Repositories;
using Minicommerce.Infrastructure.Data;

namespace Minicommerce.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && e.DeletedAt == null, cancellationToken);
    }
    public IQueryable<T> GetQueryable()
{
    return _context.Set<T>().Where(e => e.DeletedAt == null); // Exclude soft deleted
}

    public virtual async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id && e.DeletedAt == null);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => e.DeletedAt == null).ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.Where(e => e.DeletedAt == null);
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => e.IsActive && e.DeletedAt == null).ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => e.DeletedAt == null).Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.Where(e => e.DeletedAt == null);
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.Where(predicate).ToListAsync();
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => e.DeletedAt == null).FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.Where(e => e.DeletedAt == null);
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => e.DeletedAt == null).AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(e => e.DeletedAt == null);
        
        if (predicate == null)
            return await query.CountAsync(cancellationToken);
        
        return await query.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null,
        bool orderByDescending = false,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.Where(e => e.DeletedAt == null);

        // Apply includes
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        // Apply filter
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply ordering
        if (orderBy != null)
        {
            query = orderByDescending 
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }
        else
        {
            // Default ordering by CreatedAt if no ordering specified
            query = query.OrderByDescending(e => e.CreatedAt);
        }

        // Apply pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        // DeletedAt and DeletedById remain null for new entities
        
        var result = await _dbSet.AddAsync(entity, cancellationToken);
        return result.Entity;
    }
    public virtual async Task<bool> ExistsAndActiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => e.Id == id && e.IsActive && e.DeletedAt == null, cancellationToken);
    }
    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var entitiesList = entities.ToList();
        var now = DateTime.UtcNow;
        
        foreach (var entity in entitiesList)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = now;
            entity.IsActive = true;
            // DeletedAt and DeletedById remain null for new entities
        }
        
        await _dbSet.AddRangeAsync(entitiesList, cancellationToken);
        return entitiesList;
    }

    public virtual void Update(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        var now = DateTime.UtcNow;
        foreach (var entity in entities)
        {
            entity.UpdatedAt = now;
        }
        _dbSet.UpdateRange(entities);
    }

    public virtual void SoftDelete(T entity, string? deletedById = null)
    {
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedById = deletedById;
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedById = deletedById;
        _dbSet.Update(entity);
    }

    public virtual void SoftDeleteRange(IEnumerable<T> entities, string? deletedById = null)
    {
        var now = DateTime.UtcNow;
        foreach (var entity in entities)
        {
            entity.DeletedAt = now;
            entity.DeletedById = deletedById;
            entity.IsActive = false;
            entity.UpdatedAt = now;
            entity.UpdatedById = deletedById;
        }
        _dbSet.UpdateRange(entities);
    }

    public virtual void HardDelete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void HardDeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual async Task SoftDeleteByIdAsync(Guid id, string? deletedById = null, CancellationToken cancellationToken = default)
    {
        // Use FindAsync to get entity even if soft deleted, then soft delete it
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null && entity.DeletedAt == null)
        {
            SoftDelete(entity, deletedById);
        }
    }

    public virtual async Task HardDeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            HardDelete(entity);
        }
    }

    // Additional methods for handling soft deleted entities
    public virtual async Task<IEnumerable<T>> GetDeletedAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => e.DeletedAt != null).ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> GetByIdIncludeDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual void Restore(T entity)
    {
        entity.DeletedAt = null;
        entity.DeletedById = null;
        entity.IsActive = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
    }

    public virtual async Task RestoreByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null && entity.DeletedAt != null)
        {
            Restore(entity);
        }
    }
}