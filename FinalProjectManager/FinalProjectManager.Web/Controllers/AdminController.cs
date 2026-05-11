using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Data.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectManager.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> PendingApprovals()
    {
        var users = await _userManager.Users.Where(u => !u.IsApproved).ToListAsync();
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        user.IsApproved = true;
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(AppRoles.Student))
            {
                if (!await _context.Students.AnyAsync(s => s.Email == user.Email))
                {
                    _context.Students.Add(new Student { FullName = user.FullName, Email = user.Email! });
                    await _context.SaveChangesAsync();
                }
            }
            else if (roles.Contains(AppRoles.Supervisor))
            {
                if (!await _context.Supervisors.AnyAsync(s => s.Email == user.Email))
                {
                    _context.Supervisors.Add(new Supervisor { FullName = user.FullName, Email = user.Email! });
                    await _context.SaveChangesAsync();
                }
            }
        }

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

    public async Task<IActionResult> Users(string? roleFilter)
    {
        var query = _userManager.Users.Include(u => u.Specialization).AsQueryable();
        
        var users = await query.ToListAsync();
        var userList = new List<UserViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (string.IsNullOrEmpty(roleFilter) || roles.Contains(roleFilter))
            {
                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    Roles = roles.ToList(),
                    IsApproved = user.IsApproved,
                    Specialization = user.Specialization?.Name,
                    ClassDivision = user.ClassDivision
                });
            }
        }

        ViewBag.RoleFilter = roleFilter;
        return View(userList);
    }

    public async Task<IActionResult> DiplomaProjects()
    {
        var topics = await _context.Topics
            .Include(t => t.AssignedStudent)
            .ToListAsync();
        return View(topics);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAdmin(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains(AppRoles.Admin))
        {
            await _userManager.RemoveFromRoleAsync(user, AppRoles.Admin);
        }
        else
        {
            await _userManager.AddToRoleAsync(user, AppRoles.Admin);
        }

        return RedirectToAction(nameof(Users));
    }
}

public class UserViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool IsApproved { get; set; }
    public string? Specialization { get; set; }
    public string? ClassDivision { get; set; }
}
