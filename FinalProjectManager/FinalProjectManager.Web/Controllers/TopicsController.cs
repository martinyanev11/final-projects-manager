using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalProjectManager.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class TopicsController : Controller
{
    private readonly ITopicService _topicService;
    private readonly ISpecialisationService _specialisationService;

    public TopicsController(ITopicService topicService, ISpecialisationService specialisationService)
    {
        _topicService = topicService;
        _specialisationService = specialisationService;
    }

    public async Task<IActionResult> Index(string? search)
    {
        ViewData["Search"] = search;
        var topics = await _topicService.GetAllAsync(search);
        return View(topics);
    }

    public async Task<IActionResult> Details(int id)
    {
        var topic = await _topicService.GetByIdAsync(id);
        if (topic == null) return NotFound();
        return View(topic);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateSpecialisationsAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Topic topic)
    {
        ModelState.Remove(nameof(Topic.Specialisation));
        ModelState.Remove(nameof(Topic.AssignedStudent));
        if (!ModelState.IsValid)
        {
            await PopulateSpecialisationsAsync(topic.SpecialisationId);
            return View(topic);
        }
        await _topicService.CreateAsync(topic);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var topic = await _topicService.GetByIdAsync(id);
        if (topic == null) return NotFound();
        await PopulateSpecialisationsAsync(topic.SpecialisationId);
        return View(topic);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Topic topic)
    {
        if (id != topic.Id) return BadRequest();
        ModelState.Remove(nameof(Topic.Specialisation));
        ModelState.Remove(nameof(Topic.AssignedStudent));
        if (!ModelState.IsValid)
        {
            await PopulateSpecialisationsAsync(topic.SpecialisationId);
            return View(topic);
        }
        var existing = await _topicService.GetByIdAsync(id);
        if (existing == null) return NotFound();

        existing.Title = topic.Title;
        existing.Description = topic.Description;
        existing.SpecialisationId = topic.SpecialisationId;

        await _topicService.UpdateAsync(existing);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var topic = await _topicService.GetByIdAsync(id);
        if (topic == null) return NotFound();
        return View(topic);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _topicService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateSpecialisationsAsync(int? selectedId = null)
    {
        var specialisations = await _specialisationService.GetAllAsync();
        ViewBag.Specialisations = new SelectList(specialisations, "Id", "Name", selectedId);
    }
}
