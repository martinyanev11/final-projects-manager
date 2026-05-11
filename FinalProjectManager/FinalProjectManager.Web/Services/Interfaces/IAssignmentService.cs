using FinalProjectManager.Data.Models;

namespace FinalProjectManager.Web.Services.Interfaces;

public interface IAssignmentService
{
    Task<IEnumerable<Student>> GetStudentsWithTopicsAsync();
    Task<IEnumerable<Topic>> GetAvailableTopicsForStudentAsync(int studentId);
    /// <summary>Randomly assigns one unique topic to every unassigned student.</summary>
    /// <returns>Number of assignments made.</returns>
    Task<int> AutoAssignTopicsAsync();
    Task AssignTopicAsync(int studentId, int? topicId);
}
