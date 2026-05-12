using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Configuration;
using FinalProjectManager.Web.Services.Interfaces;
using FinalProjectManager.Web.ViewModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FinalProjectManager.Web.Controllers;

public class SupervisorRegistrationController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;
    private readonly EmailSettings _emailSettings;
    private readonly IConfiguration _configuration;

    public SupervisorRegistrationController(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        INotificationService notificationService,
        IOptions<EmailSettings> emailSettings,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _emailService = emailService;
        _notificationService = notificationService;
        _emailSettings = emailSettings.Value;
        _configuration = configuration;
    }

    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(SupervisorRegisterViewModel model)
    {
        // Email domain check
        var allowedDomain = _configuration["AllowedEmailDomain"] ?? "buditel.bg";
        if (!model.Email.EndsWith("@" + allowedDomain, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.Email), $"Само имейл адреси с домейн @{allowedDomain} са разрешени.");
        }

        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            IsApproved = false,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, AppRoles.Supervisor);

        // Notify admins in-app
        await _notificationService.NotifyAdminsAsync(
            "Нова заявка за регистрация на учител",
            $"{model.FullName} ({model.Email}) иска да се регистрира като учител. Прегледайте заявката в панела за одобрения.",
            "/Admin/PendingApprovals");

        // Also send email notification
        await _emailService.SendEmailAsync(
            _emailSettings.AdminEmail,
            "Нова заявка за регистрация на учител",
            $"""
            <p>Нов учител чака одобрение:</p>
            <ul>
                <li><strong>Име:</strong> {model.FullName}</li>
                <li><strong>Имейл:</strong> {model.Email}</li>
            </ul>
            <p>Влезте в <a href="{Request.Scheme}://{Request.Host}/Admin/PendingApprovals">администраторския панел</a>, за да прегледате заявката.</p>
            """);

        return RedirectToAction(nameof(PendingApproval));
    }

    public IActionResult PendingApproval() => View();
}
