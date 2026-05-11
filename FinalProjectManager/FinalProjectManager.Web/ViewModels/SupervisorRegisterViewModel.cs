using System.ComponentModel.DataAnnotations;

namespace FinalProjectManager.Web.ViewModels;

public class SupervisorRegisterViewModel
{
    [Required, MaxLength(100), Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password)), Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
