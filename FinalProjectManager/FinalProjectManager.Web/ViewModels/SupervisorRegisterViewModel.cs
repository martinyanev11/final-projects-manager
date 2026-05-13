using System.ComponentModel.DataAnnotations;

namespace FinalProjectManager.Web.ViewModels;

public class SupervisorRegisterViewModel
{
    [Required, MaxLength(100), Display(Name = "Имена")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password)), Display(Name = "Потвърди парола")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
