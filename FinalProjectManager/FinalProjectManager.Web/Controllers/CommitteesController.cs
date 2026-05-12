using FinalProjectManager.Data.Constants;
using FinalProjectManager.Web.Services.Interfaces;
using FinalProjectManager.Web.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectManager.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class CommitteesController : Controller
{
    private readonly ICommitteeService _committeeService;

    public CommitteesController(ICommitteeService committeeService)
    {
        _committeeService = committeeService;
    }

    public async Task<IActionResult> Index()
    {
        var committees = await _committeeService.GetAllAsync();
        return View(committees);
    }

    public IActionResult Generate() => View(new GenerateCommitteesViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(GenerateCommitteesViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            await _committeeService.GenerateAsync(vm.CommitteeCount, vm.SupervisorsPerCommittee);
            TempData["Success"] = $"Successfully generated {vm.CommitteeCount} committee(s).";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var committee = await _committeeService.GetByIdAsync(id);
        if (committee == null) return NotFound();
        return View(committee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetChair(int committeeId, int supervisorId)
    {
        await _committeeService.SetChairAsync(committeeId, supervisorId);
        TempData["Success"] = "Chair updated.";
        return RedirectToAction(nameof(Details), new { id = committeeId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ClearAll()
    {
        await _committeeService.ClearAllAsync();
        TempData["Success"] = "All committees cleared.";
        return RedirectToAction(nameof(Index));
    }
}
