using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Minicommerce.Domain.Common;
using Minicommerce.Domain.Entities;
using Minicommerce.Domain.Entities.User;
using Minicommerce.Domain.Repositories;
using Minicommerce.Infrastructure.Data;

namespace Minicommerce.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ConcurrentDictionary<Type, object> _repositories;
    private IDbContextTransaction? _transaction;

    // Lazy initialization for specific repositories
    private IUserRepository? _users;

    public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
        _repositories = new ConcurrentDictionary<Type, object>();
    }

    // Specific repositories with lazy initialization
    public IUserRepository Users => _users ??= new UserRepository(_context, _userManager);
    

    // Generic repository factory method
    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        return (IGenericRepository<T>)_repositories.GetOrAdd(
            typeof(T), 
            _ => new GenericRepository<T>(_context)
        );
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        _repositories.Clear();
    }
}