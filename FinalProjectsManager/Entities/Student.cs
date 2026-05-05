namespace FinalProjectsManager.Entities;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FacultyNumber { get; set; } = string.Empty;

    public int? AssignmentId { get; set; }
    public Assignment? Assignment { get; set; }
}
