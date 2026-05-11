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
    private readonly EmailSettings _emailSettings;

    public SupervisorRegistrationController(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IOptions<EmailSettings> emailSettings)
    {
        _userManager = userManager;
        _emailService = emailService;
        _emailSettings = emailSettings.Value;
    }

    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(SupervisorRegisterViewModel model)
    {
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

        await _emailService.SendEmailAsync(
            _emailSettings.AdminEmail,
            "New Supervisor Registration Awaiting Approval",
            $"""
            <p>A new supervisor has registered and is awaiting approval:</p>
            <ul>
                <li><strong>Name:</strong> {model.FullName}</li>
                <li><strong>Email:</strong> {model.Email}</li>
            </ul>
            <p>Please log in to the <a href="{Request.Scheme}://{Request.Host}/Admin/PendingApprovals">admin panel</a> to review the request.</p>
            """);

        return RedirectToAction(nameof(PendingApproval));
    }

    public IActionResult PendingApproval() => View();
}
