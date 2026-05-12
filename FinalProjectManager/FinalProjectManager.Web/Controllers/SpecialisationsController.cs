using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectManager.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class SpecialisationsController : Controller
{
    private readonly ISpecialisationService _specialisationService;

    public SpecialisationsController(ISpecialisationService specialisationService)
    {
        _specialisationService = specialisationService;
    }

    public async Task<IActionResult> Index()
    {
        var specialisations = await _specialisationService.GetAllAsync();
        return View(specialisations);
    }

    public async Task<IActionResult> Details(int id)
    {
        var specialisation = await _specialisationService.GetByIdAsync(id);
        if (specialisation == null) return NotFound();
        return View(specialisation);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Specialisation specialisation)
    {
        if (!ModelState.IsValid) return View(specialisation);
        await _specialisationService.CreateAsync(specialisation);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var specialisation = await _specialisationService.GetByIdAsync(id);
        if (specialisation == null) return NotFound();
        return View(specialisation);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Specialisation specialisation)
    {
        if (id != specialisation.Id) return BadRequest();
        if (!ModelState.IsValid) return View(specialisation);
        if (!await _specialisationService.ExistsAsync(id)) return NotFound();
        await _specialisationService.UpdateAsync(specialisation);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var specialisation = await _specialisationService.GetByIdAsync(id);
        if (specialisation == null) return NotFound();
        return View(specialisation);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (await _specialisationService.HasStudentsAsync(id))
        {
            TempData["Error"] = "Не може да се изтрие специализация с назначени ученици.";
            return RedirectToAction(nameof(Index));
        }
        await _specialisationService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
