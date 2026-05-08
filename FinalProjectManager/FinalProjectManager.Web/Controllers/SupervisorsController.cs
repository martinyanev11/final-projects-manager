using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FinalProjectManager.Web.Controllers;

public class SupervisorsController : Controller
{
    private readonly ISupervisorService _supervisorService;

    public SupervisorsController(ISupervisorService supervisorService)
    {
        _supervisorService = supervisorService;
    }

    public async Task<IActionResult> Index(string? search)
    {
        ViewData["Search"] = search;
        var supervisors = await _supervisorService.GetAllAsync(search);
        return View(supervisors);
    }

    public async Task<IActionResult> Details(int id)
    {
        var supervisor = await _supervisorService.GetByIdAsync(id);
        if (supervisor == null) return NotFound();
        return View(supervisor);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Supervisor supervisor)
    {
        if (!ModelState.IsValid) return View(supervisor);
        await _supervisorService.CreateAsync(supervisor);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var supervisor = await _supervisorService.GetByIdAsync(id);
        if (supervisor == null) return NotFound();
        return View(supervisor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Supervisor supervisor)
    {
        if (id != supervisor.Id) return BadRequest();
        if (!ModelState.IsValid) return View(supervisor);
        if (!await _supervisorService.ExistsAsync(id)) return NotFound();
        await _supervisorService.UpdateAsync(supervisor);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var supervisor = await _supervisorService.GetByIdAsync(id);
        if (supervisor == null) return NotFound();
        return View(supervisor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _supervisorService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
