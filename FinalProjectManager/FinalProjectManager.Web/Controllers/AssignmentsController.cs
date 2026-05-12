using FinalProjectManager.Data.Constants;
using FinalProjectManager.Web.Services.Interfaces;
using FinalProjectManager.Web.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalProjectManager.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AssignmentsController : Controller
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentsController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    public async Task<IActionResult> Index()
    {
        var students = await _assignmentService.GetStudentsWithTopicsAsync();
        return View(students);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AutoAssign()
    {
        try
        {
            var count = await _assignmentService.AutoAssignTopicsAsync();
            TempData["Success"] = count == 0
                ? "Всички ученици вече имат назначена тема."
                : $"Успешно назначени теми на {count} ученик/ученика.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AutoAssignSupervisors()
    {
        try
        {
            var count = await _assignmentService.AutoAssignSupervisorsAsync();
            TempData["Success"] = count == 0
                ? "Всички ученици вече имат назначен ръководител."
                : $"Успешно назначени ръководители на {count} ученик/ученика.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AutoAssignReviewers()
    {
        try
        {
            var count = await _assignmentService.AutoAssignReviewersAsync();
            TempData["Success"] = count == 0
                ? "Всички ученици вече имат назначен рецензент."
                : $"Успешно назначени рецензенти на {count} ученик/ученика.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> EditTopic(int studentId)
    {
        var topics = await _assignmentService.GetAvailableTopicsForStudentAsync(studentId);
        var students = await _assignmentService.GetStudentsWithTopicsAsync();
        var student = students.FirstOrDefault(s => s.Id == studentId);

        if (student == null) return NotFound();

        var vm = new TopicAssignmentViewModel
        {
            StudentId = studentId,
            StudentName = student.FullName,
            TopicId = student.TopicId,
            AvailableTopics = topics.Select(t => new SelectListItem(t.Title, t.Id.ToString()))
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTopic(TopicAssignmentViewModel vm)
    {
        try
        {
            await _assignmentService.AssignTopicAsync(vm.StudentId, vm.TopicId);
            TempData["Success"] = "Темата е актуализирана успешно.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            var topics = await _assignmentService.GetAvailableTopicsForStudentAsync(vm.StudentId);
            vm.AvailableTopics = topics.Select(t => new SelectListItem(t.Title, t.Id.ToString()));
            return View(vm);
        }
    }
}
