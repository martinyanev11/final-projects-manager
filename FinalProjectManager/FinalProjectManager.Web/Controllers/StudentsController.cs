using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalProjectManager.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class StudentsController : Controller
{
    private static readonly string[] BulgarianClassLetters =
    [
        "А", "Б", "В", "Г", "Д", "Е", "Ж", "З", "И", "Й",
        "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У",
        "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ъ", "Ю", "Я"
    ];

    private readonly IStudentService _studentService;
    private readonly ISpecialisationService _specialisationService;

    public StudentsController(IStudentService studentService, ISpecialisationService specialisationService)
    {
        _studentService = studentService;
        _specialisationService = specialisationService;
    }

    public async Task<IActionResult> Index(string? search)
    {
        ViewData["Search"] = search;
        var students = await _studentService.GetAllAsync(search);
        return View(students);
    }

    public async Task<IActionResult> Details(int id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null) return NotFound();
        return View(student);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Student student)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(student.SpecialisationId, student.ClassName);
            return View(student);
        }
        await _studentService.CreateAsync(student);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null) return NotFound();
        await PopulateDropdownsAsync(student.SpecialisationId, student.ClassName);
        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Student student)
    {
        if (id != student.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(student.SpecialisationId, student.ClassName);
            return View(student);
        }
        var existing = await _studentService.GetByIdAsync(id);
        if (existing == null) return NotFound();

        existing.FullName = student.FullName;
        existing.Email = student.Email;
        existing.SpecialisationId = student.SpecialisationId;
        existing.ClassName = student.ClassName;

        await _studentService.UpdateAsync(existing);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null) return NotFound();
        return View(student);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _studentService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(int? selectedSpecialisationId = null, string? selectedClassName = null)
    {
        var specialisations = await _specialisationService.GetAllAsync();
        ViewBag.Specialisations = new SelectList(specialisations, "Id", "Name", selectedSpecialisationId);
        ViewBag.ClassNames = new SelectList(BulgarianClassLetters, selectedClassName);
    }
}
