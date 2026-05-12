using System.ComponentModel.DataAnnotations;

using FinalProjectManager.Data.Constants;

namespace FinalProjectManager.Web.ViewModels;

public class StudentRegisterViewModel
{
    [Required, MaxLength(100), Display(Name = "Full Name")]
    [RegularExpression(ValidationPatterns.BulgarianName, ErrorMessage = ValidationPatterns.BulgarianNameMessage)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password)), Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required, Display(Name = "Specialisation")]
    public int SpecialisationId { get; set; }

    [Required, MaxLength(2), Display(Name = "Class")]
    public string ClassName { get; set; } = string.Empty;
}
