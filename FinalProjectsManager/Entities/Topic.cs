namespace FinalProjectsManager.Entities;

public class Topic
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;

    public int SupervisorId { get; set; }
    public Supervisor Supervisor { get; set; } = null!;
    public Assignment? Assignment { get; set; }
}
