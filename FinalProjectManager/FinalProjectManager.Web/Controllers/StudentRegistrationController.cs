using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Data;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;
using FinalProjectManager.Web.ViewModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalProjectManager.Web.Controllers;

public class StudentRegistrationController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ISpecialisationService _specialisationService;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public StudentRegistrationController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ISpecialisationService specialisationService,
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _specialisationService = specialisationService;
        _context = context;
        _configuration = configuration;
    }

    public async Task<IActionResult> Register()
    {
        await PopulateDropdownsAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(StudentRegisterViewModel model)
    {
        // Email domain check
        var allowedDomain = _configuration["AllowedEmailDomain"] ?? "buditel.bg";
        if (!model.Email.EndsWith("@" + allowedDomain, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.Email), $"Само имейл адреси с домейн @{allowedDomain} са разрешени.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model.SpecialisationId, model.ClassName);
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            IsApproved = true,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            await PopulateDropdownsAsync(model.SpecialisationId, model.ClassName);
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, AppRoles.Student);

        _context.Students.Add(new Student
        {
            FullName = model.FullName,
            Email = model.Email,
            SpecialisationId = model.SpecialisationId,
            ClassName = model.ClassName
        });
        await _context.SaveChangesAsync();

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    private static readonly string[] BulgarianClassLetters =
        ["А", "Б", "В", "Г", "Д", "Е", "Ж", "З", "И", "Й", "К", "Л", "М",
         "Н", "О", "П", "Р", "С", "Т", "У", "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ю", "Я"];

    private async Task PopulateDropdownsAsync(int? selectedSpec = null, string? selectedClass = null)
    {
        var specialisations = await _specialisationService.GetAllAsync();
        ViewBag.Specialisations = new SelectList(specialisations, "Id", "Name", selectedSpec);
        ViewBag.ClassNames = BulgarianClassLetters
            .Select(l => new SelectListItem(l, l, l == selectedClass))
            .ToList();
    }
}
