using FinalProjectManager.Data.Models;

namespace FinalProjectManager.Web.Services.Interfaces;

public interface ISpecialisationService
{
    Task<IEnumerable<Specialisation>> GetAllAsync();
    Task<Specialisation?> GetByIdAsync(int id);
    Task CreateAsync(Specialisation specialisation);
    Task UpdateAsync(Specialisation specialisation);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> HasStudentsAsync(int id);
}
