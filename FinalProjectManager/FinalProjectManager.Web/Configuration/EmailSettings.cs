namespace FinalProjectManager.Web.Configuration;

public class EmailSettings
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SenderEmail { get; set; } = "noreply@fpm.local";
    public string SenderName { get; set; } = "Final Project Manager";
    public string AdminEmail { get; set; } = "admin@fpm.local";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
}
