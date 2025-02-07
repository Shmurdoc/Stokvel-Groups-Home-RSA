using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Stokvel_Groups_Home_RSA.Areas.Identity.Pages.Role
{
    public class RoleSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RoleSeeder> _logger;

        public RoleSeeder(RoleManager<IdentityRole> roleManager, ILogger<RoleSeeder> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task CreateRolesAsync()
        {
            var roleNames = new[] { "Admin", "Manager", "SuperUser", "Member" }; // Default roles

            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Role '{roleName}' created successfully.");
                    }
                    else
                    {
                        // Log any errors that occur when creating the role
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError($"Error creating role '{roleName}': {error.Description}");
                        }
                    }
                }
                else
                {
                    _logger.LogInformation($"Role '{roleName}' already exists.");
                }
            }
        }
    }
}
