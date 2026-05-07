using FinalProjectsManager.Entities;

namespace FinalProjectsManager.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Supervisors.Any()) return;

        var supervisors = new List<Supervisor>
        {
            new() { FirstName = "Ivan", LastName = "Petrov", Email = "ivan.petrov@uni.bg", Department = "Computer Science" },
            new() { FirstName = "Maria", LastName = "Georgieva", Email = "maria.georgieva@uni.bg", Department = "Software Engineering" }
        };
        context.Supervisors.AddRange(supervisors);
        context.SaveChanges();

        var topics = new List<Topic>
        {
            new() { Title = "AI-based Recommendation System", Description = "Build a recommendation engine using ML.", SupervisorId = supervisors[0].Id },
            new() { Title = "REST API with ASP.NET Core", Description = "Design and implement a RESTful API.", SupervisorId = supervisors[1].Id }
        };
        context.Topics.AddRange(topics);
        context.SaveChanges();

        var students = new List<Student>
        {
            new() { FirstName = "Georgi", LastName = "Ivanov", Email = "georgi@student.bg", FacultyNumber = "F12345" },
            new() { FirstName = "Elena", LastName = "Todorova", Email = "elena@student.bg", FacultyNumber = "F67890" }
        };
        context.Students.AddRange(students);
        context.SaveChanges();

        var reviewers = new List<Reviewer>
        {
            new() { FirstName = "Dimitar", LastName = "Hristov", Email = "dimitar@uni.bg" }
        };
        context.Reviewers.AddRange(reviewers);
        context.SaveChanges();

        var committee = new Committee
        {
            Name = "Defense Committee 2026",
            SessionDate = new DateTime(2026, 6, 15),
            Members = new List<CommitteeMember>
            {
                new() { FirstName = "Nikolay", LastName = "Stoyanov", Email = "nikolay@uni.bg" },
                new() { FirstName = "Silviya", LastName = "Kolarova", Email = "silviya@uni.bg" }
            }
        };
        context.Committees.Add(committee);
        context.SaveChanges();

        var assignments = new List<Assignment>
        {
            new() { StudentId = students[0].Id, TopicId = topics[0].Id, SupervisorId = supervisors[0].Id, ReviewerId = reviewers[0].Id, CommitteeId = committee.Id, Status = AssignmentStatus.InProgress },
            new() { StudentId = students[1].Id, TopicId = topics[1].Id, SupervisorId = supervisors[1].Id, Status = AssignmentStatus.Pending }
        };
        context.Assignments.AddRange(assignments);
        context.SaveChanges();
    }
}
