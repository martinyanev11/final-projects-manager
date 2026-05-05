namespace FinalProjectsManager.Entities;

public class CommitteeMember
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public int CommitteeId { get; set; }
    public Committee Committee { get; set; } = null!;
}
