namespace FinalProjectManager.Data.Models;

public class CommitteeMember
{
    public int Id { get; set; }

    public int CommitteeId { get; set; }
    public DefenseCommittee Committee { get; set; } = null!;

    public int SupervisorId { get; set; }
    public Supervisor Supervisor { get; set; } = null!;

    public bool IsChair { get; set; }
}
