using TaskOps.Domain.Common;
using TaskOps.Domain.Enums;

namespace TaskOps.Domain.Entities;

/// <summary>
/// Represents a unit of work within a project.
/// Named TaskItem to avoid conflict with System.Threading.Tasks.Task.
/// </summary>
public class TaskItem : BaseEntity
{
    public Guid ProjectId { get; set; }

    /// <summary>Optional: task assigned to a specific user.</summary>
    public Guid? AssigneeId { get; set; }

    /// <summary>User who created this task.</summary>
    public Guid CreatedById { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public WorkTaskStatus Status { get; set; } = WorkTaskStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.None;

    /// <summary>Optional deadline for the task.</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Order position within the current status column.
    /// Used for drag-and-drop ordering on the board.
    /// </summary>
    public int Position { get; set; } = 0;

    // Navigation properties
    public Project Project { get; set; } = null!;
    public User? Assignee { get; set; }
    public User CreatedBy { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = [];
}