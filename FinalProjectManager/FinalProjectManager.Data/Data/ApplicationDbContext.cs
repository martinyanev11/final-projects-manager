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
    public DbSet<DefenseCommittee> DefenseCommittees { get; set; }
    public DbSet<CommitteeMember> CommitteeMembers { get; set; }
    public DbSet<Notification> Notifications { get; set; }

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
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.Entity<Student>()
            .HasOne(s => s.Reviewer)
            .WithMany(sv => sv.ReviewedStudents)
            .HasForeignKey(s => s.ReviewerId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.Entity<Student>()
            .HasOne(s => s.Committee)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.CommitteeId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.Entity<CommitteeMember>()
            .HasOne(cm => cm.Committee)
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.CommitteeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CommitteeMember>()
            .HasOne(cm => cm.Supervisor)
            .WithMany()
            .HasForeignKey(cm => cm.SupervisorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Notification>()
            .HasOne(n => n.Recipient)
            .WithMany()
            .HasForeignKey(n => n.RecipientUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}