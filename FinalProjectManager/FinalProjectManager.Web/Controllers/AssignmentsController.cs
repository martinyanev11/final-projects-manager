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
                ? "All students already have a topic assigned."
                : $"Successfully assigned topics to {count} student(s).";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // Task 9 — manual override
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
            TempData["Success"] = "Topic assignment updated.";
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
