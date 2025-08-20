using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minicommerce.Domain.Entities.User;

namespace Minicommerce.Infrastructure.Data.Seed;

public class UserSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<UserSeeder> _logger;

    public UserSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<UserSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedUsersAsync()
    {
        const string defaultPassword = "Admin@123";

        // Ensure roles exist (defensive; RoleSeeder should have done this already)
        foreach (var role in new[] { Roles.Admin, Roles.GeneralManager, Roles.DataEntry, Roles.Customer })
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
        }

        var seedUsers = new[]
        {
            new { UserName = "admin",            FullName = "System Administrator",Position = "System Administrator", Role = Roles.Admin },
            new { UserName = "admin2",           FullName = "Backup Administrator",Position = "System Administrator", Role = Roles.Admin },

            new { UserName = "general.manager",  FullName = "Ahmed Al-Rashid", Position = "General Manager",      Role = Roles.GeneralManager },
            new { UserName = "general.manager2", FullName = "Sara Al-Hakim",   Position = "General Manager",      Role = Roles.GeneralManager },

            new { UserName = "data.entry",       FullName = "Ali Kareem",  Position = "Data Entry",           Role = Roles.DataEntry },
            new { UserName = "data.entry2",      FullName = "Fatima Noor",  Position = "Data Entry",           Role = Roles.DataEntry },

            new { UserName = "customer",         FullName = "Omar Hasan", Position = "Customer",             Role = Roles.Customer },
            new { UserName = "customer2",        FullName = "Lina Jawad", Position = "Customer",             Role = Roles.Customer },
        };

        foreach (var su in seedUsers)
        {
            try
            {
                // Skip if exists
                var existing = await _userManager.FindByNameAsync(su.UserName);
                if (existing != null)
                {
                    _logger.LogInformation("User {UserName} already exists, skipping", su.UserName);
                    // Ensure role assignment (in case role was added later)
                    if (!await _userManager.IsInRoleAsync(existing, su.Role))
                    {
                        var roleFix = await _userManager.AddToRoleAsync(existing, su.Role);
                        if (!roleFix.Succeeded)
                            _logger.LogError("Failed to ensure role {Role} for existing user {UserName}: {Errors}",
                                su.Role, su.UserName, string.Join(", ", roleFix.Errors.Select(e => e.Description)));
                    }
                    continue;
                }

                // Create new user
                var user = new ApplicationUser
                {
                    UserName = su.UserName,
                    FullName = su.FullName,
                    Position = su.Position,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await _userManager.CreateAsync(user, defaultPassword);
                if (!createResult.Succeeded)
                {
                    _logger.LogError("Failed to create user {UserName}: {Errors}",
                        su.UserName, string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    continue;
                }

                var roleResult = await _userManager.AddToRoleAsync(user, su.Role);
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Failed to assign role {Role} to user {UserName}: {Errors}",
                        su.Role, su.UserName, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    continue;
                }

                _logger.LogInformation("Created user {UserName} ({FullName}) with role {Role}",
                    su.UserName, su.FullName, su.Role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {UserName}", su.UserName);
            }
        }
    }
}

// Database initialization service
public class DatabaseInitializer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IServiceProvider serviceProvider, ILogger<DatabaseInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            // Seed roles
            var roleSeeder = new RoleSeeder(roleManager);
            await roleSeeder.SeedRolesAsync();
            _logger.LogInformation("Roles seeded successfully");

            // Seed users
            var userSeeder = new UserSeeder(userManager, roleManager, loggerFactory.CreateLogger<UserSeeder>());
            await userSeeder.SeedUsersAsync();
            _logger.LogInformation("Users seeded successfully");

            // Seed catalog (categories + products)
            var catalogSeeder = services.GetRequiredService<CatalogSeeder>();
            await catalogSeeder.SeedAsync();
            _logger.LogInformation("Catalog seeded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

}
