using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskOps.Domain.Entities;
using TaskOps.Domain.Enums;

namespace TaskOps.Infrastructure.Persistence.Configurations;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id");

        builder.Property(t => t.ProjectId)
            .HasColumnName("project_id");

        builder.Property(t => t.AssignedId)
            .HasColumnName("assignee_id");

        builder.Property(t => t.CreatedById)
            .HasColumnName("created_by_id");

        builder.Property(t => t.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasColumnName("description")
            .HasMaxLength(4000);

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(WorkTaskStatus.Todo)
            .HasSentinel(WorkTaskStatus.Backlog);

        builder.Property(t => t.Priority)
            .HasColumnName("priority")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(TaskPriority.None);

        builder.Property(t => t.DueDate)
            .HasColumnName("due_date");

        builder.Property(t => t.Position)
            .HasColumnName("position")
            .HasDefaultValue(0);

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at");

        // Index for fast board queries: all tasks in a project by status
        builder.HasIndex(t => new { t.ProjectId, t.Status })
            .HasDatabaseName("ix_tasks_project_status");

        // Index for fast assignee queries: all tasks assigned to a user
        builder.HasIndex(t => t.AssignedId)
            .HasDatabaseName("ix_tasks_assignee");

        builder.HasOne(t => t.Assigned)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.CreatedBy)
            .WithMany()
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Comments)
            .WithOne(c => c.Task)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}