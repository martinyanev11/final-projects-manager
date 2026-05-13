namespace FinalProjectManager.Data.Models;

public class DefenseCommittee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<CommitteeMember> Members { get; set; } = [];
    public ICollection<Student> Students { get; set; } = [];
}
