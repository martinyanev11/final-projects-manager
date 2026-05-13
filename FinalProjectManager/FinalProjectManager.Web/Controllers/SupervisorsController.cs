using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalProjectManager.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class SupervisorsController : Controller
{
    private readonly ISupervisorService _supervisorService;
    private readonly ISpecialisationService _specialisationService;

    public SupervisorsController(ISupervisorService supervisorService, ISpecialisationService specialisationService)
    {
        _supervisorService = supervisorService;
        _specialisationService = specialisationService;
    }

    public async Task<IActionResult> Index(string? search, int? specialisationId)
    {
        ViewData["Search"] = search;
        ViewData["SelectedSpecialisation"] = specialisationId;

        var specialisations = await _specialisationService.GetAllAsync();
        ViewBag.FilterSpecialisations = new SelectList(specialisations, "Id", "Name", specialisationId);

        var supervisors = await _supervisorService.GetAllAsync(search, specialisationId);
        return View(supervisors);
    }

    public async Task<IActionResult> Details(int id)
    {
        var supervisor = await _supervisorService.GetByIdAsync(id);
        if (supervisor == null) return NotFound();
        return View(supervisor);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateSpecialisationsAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Supervisor supervisor)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSpecialisationsAsync(supervisor.SpecialisationId);
            return View(supervisor);
        }
        await _supervisorService.CreateAsync(supervisor);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var supervisor = await _supervisorService.GetByIdAsync(id);
        if (supervisor == null) return NotFound();
        await PopulateSpecialisationsAsync(supervisor.SpecialisationId);
        return View(supervisor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Supervisor supervisor)
    {
        if (id != supervisor.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            await PopulateSpecialisationsAsync(supervisor.SpecialisationId);
            return View(supervisor);
        }
        var existing = await _supervisorService.GetByIdAsync(id);
        if (existing == null) return NotFound();

        existing.FullName = supervisor.FullName;
        existing.Email = supervisor.Email;
        existing.SpecialisationId = supervisor.SpecialisationId;

        await _supervisorService.UpdateAsync(existing);
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

    private async Task PopulateSpecialisationsAsync(int? selectedId = null)
    {
        var specialisations = await _specialisationService.GetAllAsync();
        ViewBag.Specialisations = new SelectList(specialisations, "Id", "Name", selectedId);
    }
}
