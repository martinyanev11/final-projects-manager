using FinalProjectManager.Data.Models;

namespace FinalProjectManager.Web.Services.Interfaces;

public interface ITopicService
{
    Task<IEnumerable<Topic>> GetAllAsync(string? search = null);
    Task<Topic?> GetByIdAsync(int id);
    Task CreateAsync(Topic topic);
    Task UpdateAsync(Topic topic);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
