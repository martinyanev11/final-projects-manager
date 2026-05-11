using FinalProjectManager.Data.Data;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinalProjectManager.Web.Services;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _context;

    public StudentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Student>> GetAllAsync(string? search = null)
    {
        var query = _context.Students.Include(s => s.Specialisation).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.FullName.Contains(search) || s.Email.Contains(search));

        return await query.OrderBy(s => s.FullName).ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
        => await _context.Students.Include(s => s.Specialisation).FirstOrDefaultAsync(s => s.Id == id);

    public async Task CreateAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Students.AnyAsync(s => s.Id == id);
}
