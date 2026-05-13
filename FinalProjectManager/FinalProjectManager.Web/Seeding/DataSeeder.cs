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

        // Ensure "Програмиране" specialisation exists
        var spec = await context.Specialisations.FirstOrDefaultAsync(s => s.Name == "Програмиране");
        if (spec == null)
        {
            spec = new Specialisation { Name = "Програмиране" };
            context.Specialisations.Add(spec);
            await context.SaveChangesAsync();
        }

        // 35 students in Програмиране
        for (int i = 1; i <= 35; i++)
        {
            var email = $"student{i}@buditel.bg";
            if (await userManager.FindByEmailAsync(email) != null) continue;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = $"Ученик {i}",
                IsApproved = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, StudentPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, AppRoles.Student);
                context.Students.Add(new Student
                {
                    FullName = $"Ученик {i}",
                    Email = email,
                    SpecialisationId = spec.Id,
                    ClassName = "А"
                });
            }
        }
        await context.SaveChangesAsync();

        // 16 approved teachers (first 5 eligible to lead committee)
        for (int i = 1; i <= 16; i++)
        {
            var email = $"teacher{i}@buditel.bg";
            if (await userManager.FindByEmailAsync(email) != null) continue;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = $"Учител {i}",
                IsApproved = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, TeacherPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, AppRoles.Supervisor);

                if (!await context.Supervisors.AnyAsync(sv => sv.Email == email))
                {
                    context.Supervisors.Add(new Supervisor
                    {
                        FullName = $"Учител {i}",
                        Email = email,
                        SpecialisationId = spec.Id
                    });
                }
            }
        }
        await context.SaveChangesAsync();
    }
}
