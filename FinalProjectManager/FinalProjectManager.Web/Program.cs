using FinalProjectManager.Data.Data;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// 1. DATABASE & IDENTITY SERVICES
// =============================================

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Use our custom ApplicationUser throughout Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = false; // Set to true if you add email confirmation later

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddRoles<IdentityRole>()              // Enables role management
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// =============================================
// 2. MVC & RAZOR PAGES
// =============================================

builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Required for Identity UI scaffolded pages

// =============================================
// 3. PIPELINE
// =============================================

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Must come BEFORE UseAuthorization
app.UseAuthorization();

// Default MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Required for Identity Razor Pages (/Account/Login, /Account/Register, etc.)
app.MapRazorPages();

app.Run();