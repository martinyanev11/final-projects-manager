using FinalProjectManager.Data.Models;

namespace FinalProjectManager.Web.Services.Interfaces;

public interface IAssignmentService
{
    Task<IEnumerable<Student>> GetStudentsWithTopicsAsync();
    Task<IEnumerable<Topic>> GetAvailableTopicsForStudentAsync(int studentId);
    Task<int> AutoAssignTopicsAsync();
    Task AssignTopicAsync(int studentId, int? topicId);
    Task<int> AutoAssignSupervisorsAsync();
    Task<int> AutoAssignReviewersAsync();
}
