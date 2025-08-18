using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Minicommerce.Domain.Entities.User;
using Minicommerce.Domain.Repositories;
using Minicommerce.Infrastructure.Data;

namespace Minicommerce.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }
// Implementation for your UserRepository class
public async Task<IEnumerable<ApplicationUser>> GetInactiveUsersAsync(CancellationToken cancellationToken = default)
{
    return await _context.Users
        .Where(u => !u.IsActive)
        .ToListAsync(cancellationToken);
}

    public async Task<IEnumerable<ApplicationUser>> GetUsersByActiveStatusAsync(bool isActive, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.IsActive == isActive)
            .ToListAsync(cancellationToken);
    }
public void Update(ApplicationUser user)
{
    _context.Users.Update(user);
}

    public async Task<IEnumerable<ApplicationUser>> GetByRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
        return usersInRole.Where(u => u.IsActive);
    }



    public async Task<IEnumerable<ApplicationUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ApplicationUser>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<ApplicationUser> AddAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var result = await _context.Users.AddAsync(user, cancellationToken);
        return result.Entity;
    }

    public Task UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(user);
        return Task.CompletedTask;
    }
}