using Minicommerce.Domain.Common;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    // Specific repositories
    IUserRepository Users { get; }
    // Generic repository access
    IGenericRepository<T> Repository<T>() where T : BaseEntity;
    
    // Transaction management
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    // Save changes
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}