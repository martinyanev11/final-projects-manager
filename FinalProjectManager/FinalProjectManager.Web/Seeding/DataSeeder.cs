using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Data;
using FinalProjectManager.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectManager.Web.Seeding;

public static class DataSeeder
{
    public const string AdminEmail = "admin@fpm.local";
    public const string AdminPassword = "Admin123!";

    public const string StudentPassword = "Uchenik1";
    public const string TeacherPassword = "Uchitel1";

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

        await SeedTestDataAsync(services);
    }

    private static async Task SeedTestDataAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Ensure default specialisations exist
        if (!await context.Specialisations.AnyAsync())
        {
            context.Specialisations.AddRange(
                new Specialisation { Name = "Програмиране" },
                new Specialisation { Name = "Мрежи" },
                new Specialisation { Name = "Бизнес" },
                new Specialisation { Name = "Графичен дизайн" }
            );
            await context.SaveChangesAsync();
        }

        var specialisations = await context.Specialisations.ToListAsync();
        string[] classNames = ["А", "Б", "В", "Г"];

        // 20 test students
        for (int i = 1; i <= 20; i++)
        {
            var email = $"testuchenik{i}@buditel.bg";
            if (await userManager.FindByEmailAsync(email) != null) continue;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = $"Тест Ученик {i}",
                IsApproved = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, StudentPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, AppRoles.Student);
                context.Students.Add(new Student
                {
                    FullName = $"Тест Ученик {i}",
                    Email = email,
                    SpecialisationId = specialisations[(i - 1) % specialisations.Count].Id,
                    ClassName = classNames[(i - 1) % classNames.Length]
                });
            }
        }
        await context.SaveChangesAsync();

        // 20 pending teachers (IsApproved = false)
        for (int i = 1; i <= 20; i++)
        {
            var email = $"testuchitel{i}@buditel.bg";
            if (await userManager.FindByEmailAsync(email) != null) continue;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = $"Тест Учител {i}",
                IsApproved = false,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, TeacherPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, AppRoles.Supervisor);
        }
    }
}
