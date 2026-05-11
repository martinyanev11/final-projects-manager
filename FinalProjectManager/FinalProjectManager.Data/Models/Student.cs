using System.ComponentModel.DataAnnotations;

namespace FinalProjectManager.Data.Models;

public class Student
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(100), EmailAddress]
    public string Email { get; set; } = string.Empty;

    public int? TopicId { get; set; }
    public Topic? Topic { get; set; }

    [Required]
    public int SpecialisationId { get; set; }
    public Specialisation Specialisation { get; set; } = null!;

    [Required, MaxLength(2)]
    public string ClassName { get; set; } = string.Empty;
}
