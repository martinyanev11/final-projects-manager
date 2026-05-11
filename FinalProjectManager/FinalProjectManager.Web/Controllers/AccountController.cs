using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Configuration;
using FinalProjectManager.Web.Services.Interfaces;
using FinalProjectManager.Web.ViewModels;
using FinalProjectManager.Data.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalProjectManager.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;
    private readonly ApplicationDbContext _context;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IOptions<EmailSettings> emailSettings,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _emailService = emailService;
        _emailSettings = emailSettings.Value;
        _context = context;
    }

    public async Task<IActionResult> Register()
    {
        ViewBag.Specializations = new SelectList(await _context.Specializations.ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Specializations = new SelectList(await _context.Specializations.ToListAsync(), "Id", "Name");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            IsApproved = false,
            EmailConfirmed = true,
            SpecializationId = model.SpecializationId,
            ClassDivision = model.UserType == AppRoles.Student ? "12" + model.ClassDivision : null
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            
            ViewBag.Specializations = new SelectList(await _context.Specializations.ToListAsync(), "Id", "Name");
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, model.UserType);

        await _emailService.SendEmailAsync(
            _emailSettings.AdminEmail,
            "Нова регистрация за одобрение",
            $"""
            <p>Нов потребител се регистрира и очаква одобрение:</p>
            <ul>
                <li><strong>Име:</strong> {model.FullName}</li>
                <li><strong>Имейл:</strong> {model.Email}</li>
                <li><strong>Тип:</strong> {model.UserType}</li>
            </ul>
            <p>Моля, влезте в <a href="{Request.Scheme}://{Request.Host}/Admin/PendingApprovals">админ панела</a> за преглед.</p>
            """);

        return RedirectToAction(nameof(PendingApproval));
    }

    public IActionResult PendingApproval() => View();
}
