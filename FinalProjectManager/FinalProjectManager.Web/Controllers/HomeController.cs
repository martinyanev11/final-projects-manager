using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Data;
using FinalProjectManager.Web.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

namespace FinalProjectManager.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (!User.Identity!.IsAuthenticated)
            return View();

        var email = User.Identity.Name!;

        if (User.IsInRole(AppRoles.Student))
        {
            ViewBag.Student = await _context.Students
                .Include(s => s.Specialisation)
                .Include(s => s.Topic)
                .Include(s => s.Supervisor)
                .Include(s => s.Reviewer)
                .FirstOrDefaultAsync(s => s.Email == email);
        }
        else if (User.IsInRole(AppRoles.Supervisor))
        {
            ViewBag.Supervisor = await _context.Supervisors
                .Include(s => s.Specialisation)
                .Include(s => s.SupervisedStudents).ThenInclude(st => st.Topic)
                .Include(s => s.SupervisedStudents).ThenInclude(st => st.Specialisation)
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
