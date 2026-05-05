using FinalProjectsManager.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectsManager.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Supervisor> Supervisors => Set<Supervisor>();
    public DbSet<Reviewer> Reviewers => Set<Reviewer>();
    public DbSet<Committee> Committees => Set<Committee>();
    public DbSet<CommitteeMember> CommitteeMembers => Set<CommitteeMember>();
    public DbSet<Assignment> Assignments => Set<Assignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Student)
            .WithOne(s => s.Assignment)
            .HasForeignKey<Assignment>(a => a.StudentId);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Topic)
            .WithOne(t => t.Assignment)
            .HasForeignKey<Assignment>(a => a.TopicId);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Supervisor)
            .WithMany(s => s.Assignments)
            .HasForeignKey(a => a.SupervisorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Reviewer)
            .WithMany(r => r.Assignments)
            .HasForeignKey(a => a.ReviewerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
