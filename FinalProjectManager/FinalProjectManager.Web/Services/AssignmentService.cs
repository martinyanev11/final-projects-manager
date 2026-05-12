using FinalProjectManager.Data.Data;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinalProjectManager.Web.Services;

public class AssignmentService : IAssignmentService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public AssignmentService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<Student>> GetStudentsWithTopicsAsync() =>
        await _context.Students
            .Include(s => s.Topic)
            .Include(s => s.Specialisation)
            .Include(s => s.Supervisor)
            .Include(s => s.Reviewer)
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

        var assignedTopicsByStudent = new Dictionary<int, Topic>();

        foreach (var group in unassignedStudents.GroupBy(s => s.SpecialisationId))
        {
            var students = group.ToList();
            var specialisationName = students[0].Specialisation.Name;

            var availableTopics = await _context.Topics
                .Where(t => t.SpecialisationId == group.Key && t.AssignedStudent == null)
                .ToListAsync();

            if (availableTopics.Count < students.Count)
                throw new InvalidOperationException(
                    $"Недостатъчно теми за специализация \"{specialisationName}\": " +
                    $"{students.Count} ученик/ученика нямат тема, но само {availableTopics.Count} са налични.");

            var shuffled = availableTopics.OrderBy(_ => Guid.NewGuid()).ToList();
            for (var i = 0; i < students.Count; i++)
            {
                students[i].TopicId = shuffled[i].Id;
                assignedTopicsByStudent[students[i].Id] = shuffled[i];
            }
        }

        await _context.SaveChangesAsync();

        foreach (var student in unassignedStudents)
        {
            if (assignedTopicsByStudent.TryGetValue(student.Id, out var topic))
                await _notificationService.CreateForEmailAsync(
                    student.Email,
                    "Тема назначена",
                    $"Вашата дипломна тема е: {topic.Title}.");
        }

        return unassignedStudents.Count;
    }

    public async Task AssignTopicAsync(int studentId, int? topicId)
    {
        var student = await _context.Students.FindAsync(studentId)
            ?? throw new InvalidOperationException("Ученикът не е намерен.");

        if (topicId.HasValue)
        {
            var topic = await _context.Topics.FindAsync(topicId.Value)
                ?? throw new InvalidOperationException("Темата не е намерена.");

            if (topic.SpecialisationId != student.SpecialisationId)
                throw new InvalidOperationException("Тази тема е от друга специализация.");

            var alreadyTaken = await _context.Students
                .AnyAsync(s => s.TopicId == topicId && s.Id != studentId);

            if (alreadyTaken)
                throw new InvalidOperationException("Тази тема вече е назначена на друг ученик.");

            await _notificationService.CreateForEmailAsync(
                student.Email,
                "Тема актуализирана",
                $"Вашата дипломна тема е променена на: {topic.Title}.");
        }

        student.TopicId = topicId;
        await _context.SaveChangesAsync();
    }

    public async Task<int> AutoAssignSupervisorsAsync()
    {
        var unassignedStudents = await _context.Students
            .Include(s => s.Specialisation)
            .Where(s => s.SupervisorId == null)
            .ToListAsync();

        if (unassignedStudents.Count == 0)
            return 0;

        var assignedSupervisorByStudent = new Dictionary<int, Supervisor>();

        foreach (var group in unassignedStudents.GroupBy(s => s.SpecialisationId))
        {
            var students = group.ToList();
            var specialisationName = students[0].Specialisation.Name;

            var supervisors = await _context.Supervisors
                .Include(sv => sv.SupervisedStudents)
                .Where(sv => sv.SpecialisationId == group.Key)
                .ToListAsync();

            var supervisorById = supervisors.ToDictionary(sv => sv.Id);

            var slots = supervisors
                .SelectMany(sv => Enumerable.Repeat(sv.Id, Math.Max(0, 4 - sv.SupervisedStudents.Count)))
                .OrderBy(_ => Guid.NewGuid())
                .ToList();

            if (slots.Count < students.Count)
                throw new InvalidOperationException(
                    $"Недостатъчен капацитет на ръководители за специализация \"{specialisationName}\": " +
                    $"{students.Count} ученика се нуждаят от ръководител, но само {slots.Count} места са налични.");

            for (var i = 0; i < students.Count; i++)
            {
                students[i].SupervisorId = slots[i];
                assignedSupervisorByStudent[students[i].Id] = supervisorById[slots[i]];
            }
        }

        await _context.SaveChangesAsync();

        foreach (var student in unassignedStudents)
        {
            if (!assignedSupervisorByStudent.TryGetValue(student.Id, out var supervisor)) continue;

            await _notificationService.CreateForEmailAsync(
                student.Email,
                "Ръководител назначен",
                $"Вашият научен ръководител е: {supervisor.FullName}.");

            await _notificationService.CreateForEmailAsync(
                supervisor.Email,
                "Нов ученик",
                $"Назначен ви е ученик: {student.FullName}.");
        }

        return unassignedStudents.Count;
    }

    public async Task<int> AutoAssignReviewersAsync()
    {
        var students = await _context.Students
            .Include(s => s.Specialisation)
            .Where(s => s.ReviewerId == null && s.SupervisorId != null)
            .ToListAsync();

        if (students.Count == 0)
            return 0;

        var assignedReviewerByStudent = new Dictionary<int, Supervisor>();

        foreach (var group in students.GroupBy(s => s.SpecialisationId))
        {
            var groupStudents = group.ToList();
            var specialisationName = groupStudents[0].Specialisation.Name;

            var supervisors = await _context.Supervisors
                .Where(sv => sv.SpecialisationId == group.Key)
                .ToListAsync();

            foreach (var student in groupStudents)
            {
                var eligible = supervisors
                    .Where(sv => sv.Id != student.SupervisorId)
                    .OrderBy(_ => Guid.NewGuid())
                    .ToList();

                if (eligible.Count == 0)
                    throw new InvalidOperationException(
                        $"Няма подходящ рецензент за ученик \"{student.FullName}\" " +
                        $"в специализация \"{specialisationName}\": необходим е поне един друг учител.");

                student.ReviewerId = eligible[0].Id;
                assignedReviewerByStudent[student.Id] = eligible[0];
            }
        }

        await _context.SaveChangesAsync();

        foreach (var student in students)
        {
            if (!assignedReviewerByStudent.TryGetValue(student.Id, out var reviewer)) continue;

            await _notificationService.CreateForEmailAsync(
                student.Email,
                "Рецензент назначен",
                $"Вашият рецензент е: {reviewer.FullName}.");

            await _notificationService.CreateForEmailAsync(
                reviewer.Email,
                "Нов ученик за рецензия",
                $"Назначен ви е ученик за рецензиране: {student.FullName}.");
        }

        return students.Count;
    }
}
