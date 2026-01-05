using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.MigrateAsync();

        // Seed Default User
        var defaultUser = new IdentityUser
        {
            UserName = "admin@example.com",
            Email = "admin@example.com",
            EmailConfirmed = true
        };

        if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
        {
            await userManager.CreateAsync(defaultUser, "Password123!");
        }
    }
}
