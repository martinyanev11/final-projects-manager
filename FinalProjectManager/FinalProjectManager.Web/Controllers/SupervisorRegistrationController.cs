using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Configuration;
using FinalProjectManager.Web.Services.Interfaces;
using FinalProjectManager.Web.ViewModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace FinalProjectManager.Web.Controllers;

public class SupervisorRegistrationController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;
    private readonly ISpecialisationService _specialisationService;
    private readonly INotificationService _notificationService;

    public SupervisorRegistrationController(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IOptions<EmailSettings> emailSettings,
        ISpecialisationService specialisationService,
        INotificationService notificationService)
    {
        _userManager = userManager;
        _emailService = emailService;
        _emailSettings = emailSettings.Value;
        _specialisationService = specialisationService;
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Register()
    {
        await PopulateSpecialisationsAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(SupervisorRegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSpecialisationsAsync(model.SpecialisationId);
            return View(model);
        }

        var fullName = $"{model.FirstName} {model.MiddleName} {model.LastName}".Trim();

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = fullName,
            IsApproved = false,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            await PopulateSpecialisationsAsync(model.SpecialisationId);
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, AppRoles.Supervisor);

        await _notificationService.NotifyAdminsAsync(
            "Нова регистрация на учител",
            $"Учителят {fullName} ({model.Email}) се е регистрирал и очаква одобрение.",
            "/Admin/PendingApprovals");

        return RedirectToAction(nameof(PendingApproval));
    }

    public IActionResult PendingApproval() => View();

    private async Task PopulateSpecialisationsAsync(int? selectedId = null)
    {
        var specialisations = await _specialisationService.GetAllAsync();
        ViewBag.Specialisations = new SelectList(specialisations, "Id", "Name", selectedId);
    }
}
