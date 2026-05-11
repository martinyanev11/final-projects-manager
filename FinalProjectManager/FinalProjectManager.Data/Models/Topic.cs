using System.ComponentModel.DataAnnotations;

using FinalProjectManager.Data.Constants;

namespace FinalProjectManager.Data.Models;

public class Topic
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    [RegularExpression(ValidationPatterns.BulgarianText, ErrorMessage = ValidationPatterns.BulgarianTextMessage)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    [RegularExpression(ValidationPatterns.BulgarianTextOptional, ErrorMessage = ValidationPatterns.BulgarianTextMessage)]
    public string? Description { get; set; }

    public Student? AssignedStudent { get; set; }

    [Required]
    public int SpecialisationId { get; set; }
    public Specialisation Specialisation { get; set; } = null!;
}
