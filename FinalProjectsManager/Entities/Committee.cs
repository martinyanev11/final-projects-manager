namespace FinalProjectsManager.Entities;

public class Committee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }

    public ICollection<CommitteeMember> Members { get; set; } = new List<CommitteeMember>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
