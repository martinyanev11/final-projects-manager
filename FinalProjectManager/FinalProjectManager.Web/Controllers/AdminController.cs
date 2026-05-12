using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectManager.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;

    public AdminController(UserManager<ApplicationUser> userManager, INotificationService notificationService)
    {
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> PendingApprovals()
    {
        var supervisors = await _userManager.GetUsersInRoleAsync(AppRoles.Supervisor);
        var pending = supervisors.Where(u => !u.IsApproved).ToList();
        return View(pending);
    }

    public async Task<IActionResult> ReviewApproval(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(string userId, bool canLeadCommittee)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        user.IsApproved = true;
        user.CanLeadCommittee = canLeadCommittee;
        await _userManager.UpdateAsync(user);

        await _notificationService.CreateAsync(
            user.Id,
            "Акаунтът ви е одобрен",
            "Вашата заявка за регистрация беше одобрена от администратора. Вече можете да влезете в системата.");

        return RedirectToAction(nameof(PendingApprovals));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        await _userManager.DeleteAsync(user);

        return RedirectToAction(nameof(PendingApprovals));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCommitteeEligibility(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        user.CanLeadCommittee = !user.CanLeadCommittee;
        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(AllSupervisors));
    }

    public async Task<IActionResult> AllSupervisors()
    {
        var supervisors = await _userManager.GetUsersInRoleAsync(AppRoles.Supervisor);
        return View(supervisors.OrderBy(u => u.FullName).ToList());
    }
}
