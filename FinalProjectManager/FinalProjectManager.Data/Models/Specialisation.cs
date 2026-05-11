using System.ComponentModel.DataAnnotations;

namespace FinalProjectManager.Data.Models;

public class Specialisation
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Student> Students { get; set; } = [];
    public ICollection<Topic> Topics { get; set; } = [];
}
