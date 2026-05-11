using System.ComponentModel.DataAnnotations;

using FinalProjectManager.Data.Constants;

namespace FinalProjectManager.Data.Models;

public class Specialisation
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    [RegularExpression(ValidationPatterns.BulgarianName, ErrorMessage = ValidationPatterns.BulgarianNameMessage)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Student> Students { get; set; } = [];
    public ICollection<Topic> Topics { get; set; } = [];
}
