using Microsoft.AspNetCore.Identity;
using Stokvel_Groups_Home_RSA.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Stokvel_Groups_Home_RSA.Areas.Identity.Pages.Admin
{
    public class AdminSeeder
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminSeeder> _logger;

        public AdminSeeder(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<AdminSeeder> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task CreateAdminUserAsync()
        {
            var adminEmail = "admin@example.com";
            var adminPassword = "Admin@123"; // Ensure you use a strong password

            // Check if the admin user already exists
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                // Create the admin user if it doesn't exist
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true // If using email confirmation, set this to false and send confirmation email
                };

                var result = await _userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    // Ensure the "Admin" role exists before assigning
                    var roleExist = await _roleManager.RoleExistsAsync("Admin");
                    if (!roleExist)
                    {
                        var roleResult = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                        if (!roleResult.Succeeded)
                        {
                            foreach (var error in roleResult.Errors)
                            {
                                _logger.LogError($"Error creating role: {error.Description}");
                            }
                        }
                    }

                    // Assign the Admin role
                    await _userManager.AddToRoleAsync(adminUser, "Admin");

                    _logger.LogInformation("Admin user created and assigned the 'Admin' role.");
                }
                else
                {
                    // Log or handle errors for creating the user
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError($"Error creating admin user: {error.Description}");
                    }
                }
            }
            else
            {
                _logger.LogInformation("Admin user already exists.");
            }
        }
    }
}
