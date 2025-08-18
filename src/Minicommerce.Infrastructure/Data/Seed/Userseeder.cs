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

        var seedUsers = new[]
        {
            new
            {
                UserName = "admin",
                FullName = "System Administrator",
                GovernmentId = "ADM001",
                Position = "System Administrator",
                Role = Roles.Admin
            },
            new
            {
                UserName = "general.manager",
                FullName = "Ahmed Al-Rashid",
                GovernmentId = "GM001",
                Position = "General Manager",
                Role = Roles.GeneralManager
            },
        };

        foreach (var seedUser in seedUsers)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByNameAsync(seedUser.UserName);
                if (existingUser != null)
                {
                    _logger.LogInformation("User {UserName} already exists, skipping", seedUser.UserName);
                    continue;
                }

                // Create new user
                var user = new ApplicationUser
                {
                    UserName = seedUser.UserName,
                    FullName = seedUser.FullName,
                    Position = seedUser.Position,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                // Create user with password
                var createResult = await _userManager.CreateAsync(user, defaultPassword);
                if (!createResult.Succeeded)
                {
                    _logger.LogError("Failed to create user {UserName}: {Errors}",
                        seedUser.UserName,
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    continue;
                }

                // Assign role to user
                var roleResult = await _userManager.AddToRoleAsync(user, seedUser.Role);
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Failed to assign role {Role} to user {UserName}: {Errors}",
                        seedUser.Role,
                        seedUser.UserName,
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    continue;
                }

                _logger.LogInformation("Successfully created user {UserName} ({FullName}) with role {Role}",
                    seedUser.UserName, seedUser.FullName, seedUser.Role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {UserName}", seedUser.UserName);
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

            // Seed roles first
            var roleSeeder = new RoleSeeder(roleManager);
            await roleSeeder.SeedRolesAsync();
            _logger.LogInformation("Roles seeded successfully");

            // Then seed users
            var userSeeder = new UserSeeder(userManager, roleManager, loggerFactory.CreateLogger<UserSeeder>());
            await userSeeder.SeedUsersAsync();
            _logger.LogInformation("Users seeded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}