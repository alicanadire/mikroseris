using Microsoft.AspNetCore.Identity;
using ToyStore.IdentityService.Models;

namespace ToyStore.IdentityService;

public static class SeedData
{
    public static async Task Initialize(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Create roles
        await CreateRoles(roleManager);
        
        // Create admin user
        await CreateAdminUser(userManager);
        
        // Create test users
        await CreateTestUsers(userManager);
    }

    private static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Admin", "Manager", "Customer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task CreateAdminUser(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@toystore.com";
        
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

    private static async Task CreateTestUsers(UserManager<ApplicationUser> userManager)
    {
        var testUsers = new[]
        {
            new { Email = "manager@toystore.com", FirstName = "Store", LastName = "Manager", Role = "Manager" },
            new { Email = "customer@toystore.com", FirstName = "John", LastName = "Customer", Role = "Customer" }
        };

        foreach (var testUser in testUsers)
        {
            var user = await userManager.FindByEmailAsync(testUser.Email);
            
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = testUser.Email,
                    Email = testUser.Email,
                    FirstName = testUser.FirstName,
                    LastName = testUser.LastName,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(user, "Test123!");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, testUser.Role);
                }
            }
        }
    }
}
