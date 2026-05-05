namespace FinalProjectsManager.Entities;

public enum AssignmentStatus { Pending, InProgress, Submitted, Defended }

public class Assignment
{
    public int Id { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int TopicId { get; set; }
    public Topic Topic { get; set; } = null!;

    public int SupervisorId { get; set; }
    public Supervisor Supervisor { get; set; } = null!;

    public int? ReviewerId { get; set; }
    public Reviewer? Reviewer { get; set; }

    public int? CommitteeId { get; set; }
    public Committee? Committee { get; set; }
}
