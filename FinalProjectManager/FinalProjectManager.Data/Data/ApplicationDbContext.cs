using FinalProjectManager.Data.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectManager.Data.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Topic> Topics { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Supervisor> Supervisors { get; set; }
    public DbSet<Specialisation> Specialisations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("Users");

        builder.Entity<Student>()
            .HasOne(s => s.Topic)
            .WithOne(t => t.AssignedStudent)
            .HasForeignKey<Student>(s => s.TopicId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Student>()
            .HasOne(s => s.Specialisation)
            .WithMany(sp => sp.Students)
            .HasForeignKey(s => s.SpecialisationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Topic>()
            .HasOne(t => t.Specialisation)
            .WithMany(sp => sp.Topics)
            .HasForeignKey(t => t.SpecialisationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Supervisor>()
            .HasOne(sv => sv.Specialisation)
            .WithMany(sp => sp.Supervisors)
            .HasForeignKey(sv => sv.SpecialisationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Student>()
            .HasOne(s => s.Supervisor)
            .WithMany(sv => sv.SupervisedStudents)
            .HasForeignKey(s => s.SupervisorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Student>()
            .HasOne(s => s.Reviewer)
            .WithMany(sv => sv.ReviewedStudents)
            .HasForeignKey(s => s.ReviewerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}