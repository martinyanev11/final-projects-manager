using System.ComponentModel.DataAnnotations;

namespace FinalProjectManager.Web.ViewModels;

public class SupervisorRegisterViewModel
{
    [Required, MaxLength(50), Display(Name = "Първо Ime")]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50), Display(Name = "Презиме")]
    public string MiddleName { get; set; } = string.Empty;

    [Required, MaxLength(50), Display(Name = "Фамилия")]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(100), Display(Name = "Имейл")]
    public string Email { get; set; } = string.Empty;

    [Required, Display(Name = "Специализация")]
    public int SpecialisationId { get; set; }

    [Required, DataType(DataType.Password), MinLength(6), Display(Name = "Парола")]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password)), Display(Name = "Потвърди паролата")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
