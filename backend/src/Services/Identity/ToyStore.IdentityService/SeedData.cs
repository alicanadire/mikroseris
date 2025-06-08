using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToyStore.IdentityService.Configuration;
using ToyStore.IdentityService.Data;
using ToyStore.IdentityService.Models;

namespace ToyStore.IdentityService;

public class SeedData
{
    public static async Task EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        
        await SeedIdentityServerData(scope.ServiceProvider);
        await SeedUsers(scope.ServiceProvider);
    }

    private static async Task SeedIdentityServerData(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ConfigurationDbContext>();
        await context.Database.MigrateAsync();

        if (!context.Clients.Any())
        {
            foreach (var client in Config.Clients)
            {
                context.Clients.Add(client.ToEntity());
            }
            await context.SaveChangesAsync();
        }

        if (!context.IdentityResources.Any())
        {
            foreach (var resource in Config.IdentityResources)
            {
                context.IdentityResources.Add(resource.ToEntity());
            }
            await context.SaveChangesAsync();
        }

        if (!context.ApiScopes.Any())
        {
            foreach (var apiScope in Config.ApiScopes)
            {
                context.ApiScopes.Add(apiScope.ToEntity());
            }
            await context.SaveChangesAsync();
        }

        if (!context.ApiResources.Any())
        {
            foreach (var apiResource in Config.ApiResources)
            {
                context.ApiResources.Add(apiResource.ToEntity());
            }
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedUsers(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Create roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (!await roleManager.RoleExistsAsync("Customer"))
        {
            await roleManager.CreateAsync(new IdentityRole("Customer"));
        }

        // Create admin user
        var adminUser = await userManager.FindByEmailAsync("admin@toystore.com");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin@toystore.com",
                Email = "admin@toystore.com",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                await userManager.AddClaimsAsync(adminUser, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, $"{adminUser.FirstName} {adminUser.LastName}"),
                    new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
                    new Claim(JwtClaimTypes.Email, adminUser.Email),
                    new Claim(JwtClaimTypes.Role, "Admin")
                });
            }
        }

        // Create test customer user
        var customerUser = await userManager.FindByEmailAsync("customer@toystore.com");
        if (customerUser == null)
        {
            customerUser = new ApplicationUser
            {
                UserName = "customer@toystore.com",
                Email = "customer@toystore.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(customerUser, "Customer123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(customerUser, "Customer");
                await userManager.AddClaimsAsync(customerUser, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, $"{customerUser.FirstName} {customerUser.LastName}"),
                    new Claim(JwtClaimTypes.GivenName, customerUser.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, customerUser.LastName),
                    new Claim(JwtClaimTypes.Email, customerUser.Email),
                    new Claim(JwtClaimTypes.Role, "Customer")
                });
            }
        }
    }
}
