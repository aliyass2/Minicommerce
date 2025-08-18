using System.Linq.Expressions;
using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Repositories;

public interface IGenericRepository<T> where T : BaseEntity
{
    // Query operations (automatically exclude soft deleted)
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    IQueryable<T> GetQueryable(); // Add this method

    Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<bool> ExistsAndActiveAsync(Guid id, CancellationToken cancellationToken = default);

    // Pagination
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null,
        bool orderByDescending = false,
        params Expression<Func<T, object>>[] includes);

    // Command operations
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void SoftDelete(T entity, string? deletedById = null);
    void SoftDeleteRange(IEnumerable<T> entities, string? deletedById = null);
    void HardDelete(T entity);
    void HardDeleteRange(IEnumerable<T> entities);
    Task SoftDeleteByIdAsync(Guid id, string? deletedById = null, CancellationToken cancellationToken = default);
    Task HardDeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Soft delete specific operations
    Task<IEnumerable<T>> GetDeletedAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdIncludeDeletedAsync(Guid id, CancellationToken cancellationToken = default);
    void Restore(T entity);
    Task RestoreByIdAsync(Guid id, CancellationToken cancellationToken = default);
}