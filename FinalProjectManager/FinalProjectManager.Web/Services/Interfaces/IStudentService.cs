using FinalProjectManager.Data.Models;

namespace FinalProjectManager.Web.Services.Interfaces;

public interface IStudentService
{
    Task<IEnumerable<Student>> GetAllAsync(string? search = null);
    Task<Student?> GetByIdAsync(int id);
    Task CreateAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
