using Core.Constants;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        string defaultAdminEmail,
        string defaultAdminPassword)
    {
        await SeedRolesAsync(roleManager);
        await SeedAdminAsync(userManager, defaultAdminEmail, defaultAdminPassword);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in new[] { RoleNames.Admin, RoleNames.User })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedAdminAsync
        (UserManager<ApplicationUser> userManager,
        string email,
        string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
            return;

        var admin = new ApplicationUser
        {
            UserName = "admin",
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, password);

        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, RoleNames.Admin);
    }
}   