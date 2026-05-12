using FinalProjectManager.Data.Models;

namespace FinalProjectManager.Web.Services.Interfaces;

public interface INotificationService
{
    Task CreateAsync(string recipientUserId, string title, string body, string? link = null);
    Task CreateForEmailAsync(string recipientEmail, string title, string body, string? link = null);
    Task NotifyAdminsAsync(string title, string body, string? link = null);
    Task<IEnumerable<Notification>> GetForUserAsync(string userId);
    Task MarkReadAsync(int notificationId, string userId);
    Task MarkAllReadAsync(string userId);
    Task<int> GetUnreadCountAsync(string userId);
}
