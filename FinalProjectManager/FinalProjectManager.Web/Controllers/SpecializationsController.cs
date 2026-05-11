using FinalProjectManager.Data.Data;
using FinalProjectManager.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalProjectManager.Data.Constants;

namespace FinalProjectManager.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class SpecializationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public SpecializationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Specializations.ToListAsync());
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Specialization specialization)
    {
        if (ModelState.IsValid)
        {
            _context.Specializations.Add(specialization);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(specialization);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var specialization = await _context.Specializations.FindAsync(id);
        if (specialization == null) return NotFound();
        return View(specialization);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Specialization specialization)
    {
        if (id != specialization.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            _context.Update(specialization);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(specialization);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var specialization = await _context.Specializations.FindAsync(id);
        if (specialization == null) return NotFound();
        return View(specialization);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var specialization = await _context.Specializations.FindAsync(id);
        if (specialization != null)
        {
            _context.Specializations.Remove(specialization);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
