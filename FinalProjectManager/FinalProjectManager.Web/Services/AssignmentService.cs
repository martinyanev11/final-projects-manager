using FinalProjectManager.Data.Data;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinalProjectManager.Web.Services;

public class AssignmentService : IAssignmentService
{
    private readonly ApplicationDbContext _context;

    public AssignmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Student>> GetStudentsWithTopicsAsync() =>
        await _context.Students
            .Include(s => s.Topic)
            .Include(s => s.Specialisation)
            .OrderBy(s => s.FullName)
            .ToListAsync();

    public async Task<IEnumerable<Topic>> GetAvailableTopicsForStudentAsync(int studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null) return [];

        return await _context.Topics
            .Where(t => t.SpecialisationId == student.SpecialisationId &&
                        (t.AssignedStudent == null || t.AssignedStudent.Id == studentId))
            .OrderBy(t => t.Title)
            .ToListAsync();
    }

    public async Task<int> AutoAssignTopicsAsync()
    {
        var unassignedStudents = await _context.Students
            .Include(s => s.Specialisation)
            .Where(s => s.TopicId == null)
            .ToListAsync();

        if (unassignedStudents.Count == 0)
            return 0;

        foreach (var group in unassignedStudents.GroupBy(s => s.SpecialisationId))
        {
            var students = group.ToList();
            var specialisationName = students[0].Specialisation.Name;

            var availableTopics = await _context.Topics
                .Where(t => t.SpecialisationId == group.Key && t.AssignedStudent == null)
                .ToListAsync();

            if (availableTopics.Count < students.Count)
                throw new InvalidOperationException(
                    $"Not enough topics for specialisation \"{specialisationName}\": " +
                    $"{students.Count} student(s) need a topic but only {availableTopics.Count} are available.");

            var shuffled = availableTopics.OrderBy(_ => Guid.NewGuid()).ToList();
            for (var i = 0; i < students.Count; i++)
                students[i].TopicId = shuffled[i].Id;
        }

        await _context.SaveChangesAsync();
        return unassignedStudents.Count;
    }

    public async Task AssignTopicAsync(int studentId, int? topicId)
    {
        var student = await _context.Students.FindAsync(studentId)
            ?? throw new InvalidOperationException("Student not found.");

        if (topicId.HasValue)
        {
            var topic = await _context.Topics.FindAsync(topicId.Value)
                ?? throw new InvalidOperationException("Topic not found.");

            if (topic.SpecialisationId != student.SpecialisationId)
                throw new InvalidOperationException("That topic belongs to a different specialisation.");

            var alreadyTaken = await _context.Students
                .AnyAsync(s => s.TopicId == topicId && s.Id != studentId);

            if (alreadyTaken)
                throw new InvalidOperationException("That topic is already assigned to another student.");
        }

        student.TopicId = topicId;
        await _context.SaveChangesAsync();
    }
}
