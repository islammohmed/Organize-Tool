using Microsoft.EntityFrameworkCore;
using ClockifyData.Domain.Entities;
using DomainTask = ClockifyData.Domain.Entities.Task;

namespace ClockifyData.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets for entities
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<DomainTask> Tasks { get; set; }
    public DbSet<TimeEntry> TimeEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships explicitly (optional, but good practice)
        
        // User -> Projects (One-to-Many)
        modelBuilder.Entity<Project>()
            .HasOne(p => p.User)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Project -> Tasks (One-to-Many)
        modelBuilder.Entity<DomainTask>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // User -> Tasks (One-to-Many)
        modelBuilder.Entity<DomainTask>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Task -> TimeEntries (One-to-Many)
        modelBuilder.Entity<TimeEntry>()
            .HasOne(te => te.Task)
            .WithMany(t => t.TimeEntries)
            .HasForeignKey(te => te.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        // User -> TimeEntries (One-to-Many)
        modelBuilder.Entity<TimeEntry>()
            .HasOne(te => te.User)
            .WithMany(u => u.TimeEntries)
            .HasForeignKey(te => te.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Additional configurations
        modelBuilder.Entity<TimeEntry>()
            .ToTable(t => t.HasCheckConstraint("CK_TimeEntry_EndTime_After_StartTime", "EndTime > StartTime"));
    }
}