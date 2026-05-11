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
            .OrderBy(s => s.FullName)
            .ToListAsync();

    public async Task<IEnumerable<Topic>> GetAvailableTopicsForStudentAsync(int studentId) =>
        await _context.Topics
            .Where(t => t.AssignedStudent == null || t.AssignedStudent.Id == studentId)
            .OrderBy(t => t.Title)
            .ToListAsync();

    public async Task<int> AutoAssignTopicsAsync()
    {
        var unassignedStudents = await _context.Students
            .Where(s => s.TopicId == null)
            .ToListAsync();

        if (unassignedStudents.Count == 0)
            return 0;

        var availableTopics = await _context.Topics
            .Where(t => t.AssignedStudent == null)
            .ToListAsync();

        if (availableTopics.Count < unassignedStudents.Count)
            throw new InvalidOperationException(
                $"Not enough topics: {unassignedStudents.Count} student(s) need a topic but only {availableTopics.Count} topic(s) are unassigned.");

        // Fisher-Yates shuffle via Guid for randomness
        var shuffled = availableTopics.OrderBy(_ => Guid.NewGuid()).ToList();

        for (var i = 0; i < unassignedStudents.Count; i++)
            unassignedStudents[i].TopicId = shuffled[i].Id;

        await _context.SaveChangesAsync();
        return unassignedStudents.Count;
    }

    public async Task AssignTopicAsync(int studentId, int? topicId)
    {
        var student = await _context.Students.FindAsync(studentId)
            ?? throw new InvalidOperationException("Student not found.");

        if (topicId.HasValue)
        {
            var alreadyTaken = await _context.Students
                .AnyAsync(s => s.TopicId == topicId && s.Id != studentId);

            if (alreadyTaken)
                throw new InvalidOperationException("That topic is already assigned to another student.");
        }

        student.TopicId = topicId;
        await _context.SaveChangesAsync();
    }
}
