using FinalProjectManager.Data.Data;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services;
using FinalProjectManager.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace FinalProjectManager.Tests;

public class AssignmentServiceTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetAvailableTopicsForStudentAsync_ReturnsCorrectTopics()
    {
        // Arrange
        var context = GetDbContext();
        var emailServiceMock = new Mock<IEmailService>();
        var service = new AssignmentService(context, emailServiceMock.Object);

        var spec = new Specialisation { Name = "Програмиране" };
        var spec2 = new Specialisation { Name = "Дизайн" };
        context.Specialisations.AddRange(spec, spec2);
        
        var student = new Student { FullName = "Иван", Email = "ivan@test.com", ClassName = "12A", Specialisation = spec };
        context.Students.Add(student);
        
        var topic1 = new Topic { Title = "Topic 1", Description = "Desc", Specialisation = spec };
        var topic2 = new Topic { Title = "Topic 2", Description = "Desc", Specialisation = spec2 };
        context.Topics.AddRange(topic1, topic2);
        
        await context.SaveChangesAsync();

        // Act
        var topics = await service.GetAvailableTopicsForStudentAsync(student.Id);

        // Assert
        Assert.Single(topics);
        Assert.Equal("Topic 1", topics.First().Title);
    }
    
    [Fact]
    public async Task AssignTopicAsync_AssignsTopicAndSendsEmail()
    {
        // Arrange
        var context = GetDbContext();
        var emailServiceMock = new Mock<IEmailService>();
        var service = new AssignmentService(context, emailServiceMock.Object);

        var spec = new Specialisation { Name = "Програмиране" };
        context.Specialisations.Add(spec);
        
        var student = new Student { FullName = "Иван", Email = "ivan@test.com", ClassName = "12A", Specialisation = spec };
        context.Students.Add(student);
        
        var topic = new Topic { Title = "Topic 1", Description = "Desc", Specialisation = spec };
        context.Topics.Add(topic);
        
        await context.SaveChangesAsync();

        // Act
        await service.AssignTopicAsync(student.Id, topic.Id);

        // Assert
        var updatedStudent = await context.Students.FindAsync(student.Id);
        Assert.Equal(topic.Id, updatedStudent.TopicId);
        
        emailServiceMock.Verify(x => x.SendEmailAsync(
            student.Email, 
            It.IsAny<string>(), 
            It.Is<string>(s => s.Contains("Topic 1"))), Times.Once);
    }
}
