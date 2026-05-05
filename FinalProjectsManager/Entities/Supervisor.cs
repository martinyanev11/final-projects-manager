namespace FinalProjectsManager.Entities;

public class Supervisor
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;

    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
