using System.ComponentModel.DataAnnotations;

namespace FinalProjectManager.Data.Models;

public class Supervisor
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(100), EmailAddress]
    public string Email { get; set; } = string.Empty;
}
