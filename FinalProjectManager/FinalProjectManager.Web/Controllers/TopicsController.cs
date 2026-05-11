using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectManager.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class TopicsController : Controller
{
    private readonly ITopicService _topicService;

    public TopicsController(ITopicService topicService)
    {
        _topicService = topicService;
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

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Topic topic)
    {
        if (!ModelState.IsValid) return View(topic);
        await _topicService.CreateAsync(topic);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var topic = await _topicService.GetByIdAsync(id);
        if (topic == null) return NotFound();
        return View(topic);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Topic topic)
    {
        if (id != topic.Id) return BadRequest();
        if (!ModelState.IsValid) return View(topic);
        if (!await _topicService.ExistsAsync(id)) return NotFound();
        await _topicService.UpdateAsync(topic);
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
}
