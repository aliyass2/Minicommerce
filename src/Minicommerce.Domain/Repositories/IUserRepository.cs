using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Minicommerce.Domain.Entities.User;

namespace Minicommerce.Domain.Repositories;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> GetByRoleAsync(string roleName, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<ApplicationUser> AddAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task DeleteAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> GetInactiveUsersAsync(CancellationToken cancellationToken = default);
    
    // Alternative method if you want to be more explicit
    Task<IEnumerable<ApplicationUser>> GetUsersByActiveStatusAsync(bool isActive, CancellationToken cancellationToken = default);
    void Update(ApplicationUser user);

}
