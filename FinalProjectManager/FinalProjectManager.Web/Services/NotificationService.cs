using FinalProjectManager.Data.Data;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectManager.Web.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task CreateAsync(string recipientUserId, string title, string body, string? link = null)
    {
        _context.Notifications.Add(new Notification
        {
            RecipientUserId = recipientUserId,
            Title = title,
            Body = body,
            Link = link,
            CreatedAt = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }

    public async Task CreateForEmailAsync(string recipientEmail, string title, string body, string? link = null)
    {
        var user = await _userManager.FindByEmailAsync(recipientEmail);
        if (user != null)
            await CreateAsync(user.Id, title, body, link);
    }

    public async Task NotifyAdminsAsync(string title, string body, string? link = null)
    {
        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        foreach (var admin in admins)
            await CreateAsync(admin.Id, title, body, link);
    }

    public async Task<IEnumerable<Notification>> GetForUserAsync(string userId) =>
        await _context.Notifications
            .Where(n => n.RecipientUserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public async Task MarkReadAsync(int notificationId, string userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientUserId == userId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllReadAsync(string userId)
    {
        var unread = await _context.Notifications
            .Where(n => n.RecipientUserId == userId && !n.IsRead)
            .ToListAsync();
        foreach (var n in unread)
            n.IsRead = true;
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(string userId) =>
        await _context.Notifications
            .CountAsync(n => n.RecipientUserId == userId && !n.IsRead);
}
