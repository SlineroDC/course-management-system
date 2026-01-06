using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.MigrateAsync();

        // Seed Roles
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Seed Default Admin User
        var defaultUser = new IdentityUser
        {
            UserName = "admin@example.com",
            Email = "admin@example.com",
            EmailConfirmed = true
        };

        if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
        {
            var result = await userManager.CreateAsync(defaultUser, "Password123!");
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to seed default user: {errors}");
            }

            // Assign Admin role
            await userManager.AddToRoleAsync(defaultUser, "Admin");
        }
        else
        {
            // Ensure existing admin has Admin role
            var existingUser = await userManager.FindByEmailAsync(defaultUser.Email);
            if (existingUser != null && !await userManager.IsInRoleAsync(existingUser, "Admin"))
            {
                await userManager.AddToRoleAsync(existingUser, "Admin");
            }
        }
    }
}
