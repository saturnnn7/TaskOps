using TaskOps.Domain.Common;

namespace TaskOps.Domain.Entities;

/// <summary>
/// Represents a comment left by a user on a specific task.
/// </summary>
public class Comment : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Marks comment as deleted without removing it from the database.
    /// Soft delete — preserves thread context for other users.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public TaskItem Task { get; set; } = null!;
    public User Author { get; set; } = null!;
}