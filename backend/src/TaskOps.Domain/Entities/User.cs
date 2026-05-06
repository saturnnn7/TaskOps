using TaskOps.Domain.Common;

namespace TaskOps.Domain.Entities;

/// <summary>
/// Represents a registered user in the system.
/// Central identity entity — all data belongs to a user.
/// </summary>
public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Argon2 hashed password. Never store or return plain text.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsEmailVerified { get; set; } = false;
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public ICollection<ProjectMember> ProjectMemberships { get; set; } = [];
    public ICollection<TaskItem> AssignedTasks { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
}