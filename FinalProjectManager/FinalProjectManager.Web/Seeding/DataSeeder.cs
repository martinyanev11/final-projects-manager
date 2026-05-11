using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Models;

using Microsoft.AspNetCore.Identity;

namespace FinalProjectManager.Web.Seeding;

public static class DataSeeder
{
    public const string AdminEmail = "admin@fpm.local";
    public const string AdminPassword = "Admin123!";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { AppRoles.Admin, AppRoles.Supervisor, AppRoles.Student })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        if (await userManager.FindByEmailAsync(AdminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                FullName = "Administrator",
                IsApproved = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, AdminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, AppRoles.Admin);
        }

        var dbContext = services.GetRequiredService<FinalProjectManager.Data.Data.ApplicationDbContext>();
        if (!dbContext.Specializations.Any())
        {
            dbContext.Specializations.AddRange(
                new Specialization { Name = "Приложно програмиране" },
                new Specialization { Name = "Системно програмиране" },
                new Specialization { Name = "Компютърни мрежи" },
                new Specialization { Name = "Икономическа информатика" }
            );
            await dbContext.SaveChangesAsync();
        }
    }
}
