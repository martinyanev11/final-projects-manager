using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectManager.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> PendingApprovals()
    {
        var supervisors = await _userManager.GetUsersInRoleAsync(AppRoles.Supervisor);
        var pending = supervisors.Where(u => !u.IsApproved).ToList();
        return View(pending);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        user.IsApproved = true;
        await _userManager.UpdateAsync(user);

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

    public async Task<IActionResult> AllSupervisors()
    {
        var supervisors = await _userManager.GetUsersInRoleAsync(AppRoles.Supervisor);
        return View(supervisors.OrderBy(u => u.FullName).ToList());
    }
}
