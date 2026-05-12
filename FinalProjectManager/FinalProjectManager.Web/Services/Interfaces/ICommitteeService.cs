using FinalProjectManager.Data.Models;

namespace FinalProjectManager.Web.Services.Interfaces;

public interface ICommitteeService
{
    Task<IEnumerable<DefenseCommittee>> GetAllAsync();
    Task<DefenseCommittee?> GetByIdAsync(int id);
    Task GenerateAsync(int committeeCount, int supervisorsPerCommittee);
    Task SetChairAsync(int committeeId, int supervisorId);
    Task ClearAllAsync();
}
