using System.ComponentModel.DataAnnotations;

namespace FinalProjectManager.Data.Models;

public class Topic
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public Student? AssignedStudent { get; set; }
}
