using System.ComponentModel.DataAnnotations;

using FinalProjectManager.Data.Constants;

namespace FinalProjectManager.Data.Models;

public class Supervisor
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    [RegularExpression(ValidationPatterns.BulgarianName, ErrorMessage = ValidationPatterns.BulgarianNameMessage)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(100), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public int SpecialisationId { get; set; }
    public Specialisation Specialisation { get; set; } = null!;

    public ICollection<Student> SupervisedStudents { get; set; } = [];
    public ICollection<Student> ReviewedStudents { get; set; } = [];
}
