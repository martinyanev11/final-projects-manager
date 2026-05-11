using System.ComponentModel.DataAnnotations;

namespace FinalProjectManager.Web.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Пълното име е задължително")]
    [MaxLength(100)]
    [Display(Name = "Пълно име")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Имейлът е задължителен")]
    [EmailAddress(ErrorMessage = "Невалиден имейл")]
    [MaxLength(100)]
    [Display(Name = "Имейл")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Паролата е задължителна")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Паролата трябва да е поне 6 символа")]
    [Display(Name = "Парола")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Потвърждението на паролата е задължително")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Паролите не съвпадат")]
    [Display(Name = "Потвърди парола")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Типът потребител е задължителен")]
    [Display(Name = "Тип потребител")]
    public string UserType { get; set; } = string.Empty; // "Student" or "Supervisor"

    [Display(Name = "Паралелка")]
    public string? ClassDivision { get; set; } // A, B, V, G, D, E, J, Z, I

    [Required(ErrorMessage = "Специалността е задължителна")]
    [Display(Name = "Специалност")]
    public int SpecializationId { get; set; }
}
