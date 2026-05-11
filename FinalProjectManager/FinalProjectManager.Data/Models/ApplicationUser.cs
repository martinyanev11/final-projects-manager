using Microsoft.AspNetCore.Identity;

namespace FinalProjectManager.Data.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = true;
    public string? ClassDivision { get; set; }
    public int? SpecializationId { get; set; }
    public Specialization? Specialization { get; set; }
}