using Microsoft.AspNetCore.Identity;

namespace Minicommerce.Infrastructure.Data.Seed
{
    // Constants for role names
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string GeneralManager = "GeneralManager";
        public const string DataEntry = "DataEntry";
        public const string Customer = "Customer";
    }

    // Role seeder service
    public class RoleSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleSeeder(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedRolesAsync()
        {
            var roles = new[]
            {
                Roles.Admin,
                Roles.GeneralManager,
                Roles.DataEntry,
                Roles.Customer
            };

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    await _roleManager.CreateAsync(role);
                }
            }
        }
    }
}
