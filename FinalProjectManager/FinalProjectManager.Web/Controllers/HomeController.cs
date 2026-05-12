using FinalProjectManager.Data.Data;
using FinalProjectManager.Web.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

namespace FinalProjectManager.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Student"))
            {
                var email = User.Identity?.Name;
                var student = await _context.Students
                    .Include(s => s.Topic)
                    .Include(s => s.Supervisor)
                    .Include(s => s.Reviewer)
                    .Include(s => s.Committee)
                    .FirstOrDefaultAsync(s => s.Email == email);
                return View(student);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
