namespace FinalProjectManager.Data.Models;

public class Notification
{
    public int Id { get; set; }
    public string RecipientUserId { get; set; } = string.Empty;
    public ApplicationUser Recipient { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Link { get; set; }
}
