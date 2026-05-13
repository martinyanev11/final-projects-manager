using FinalProjectManager.Data.Models;

namespace FinalProjectManager.Web.Services.Interfaces;

public interface ISupervisorService
{
    Task<IEnumerable<Supervisor>> GetAllAsync(string? search = null, int? specialisationId = null);
    Task<Supervisor?> GetByIdAsync(int id);
    Task CreateAsync(Supervisor supervisor);
    Task UpdateAsync(Supervisor supervisor);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
