using TaskOps.Domain.Common;
using TaskOps.Domain.Enums;

namespace TaskOps.Domain.Entities;

/// <summary>
/// Join entity representing a user's membership in a project.
/// Carries the role that defines what the user can do in the project.
/// This is a many-to-many relationship between User and Project with extra data (Role).
/// </summary>
public class ProjectMember : BaseEntity
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public ProjectRole Role { get; set; } = ProjectRole.Member;

    /// <summary>When the user was invited or joined the project.</summary>
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Project Project { get; set; } = null!;
    public User User { get; set; } = null!;
}