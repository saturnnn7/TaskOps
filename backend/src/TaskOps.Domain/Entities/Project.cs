using TaskOps.Domain.Common;
using TaskOps.Domain.Enums;

namespace TaskOps.Domain.Entities;

/// <summary>
/// Represents a project that contains tasks and members.
/// A project is owned by one user but can have multiple members with different roles.
/// </summary>
public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    /// <summary>Unique URL-friendly identifier. Example: "my-project-2024"</summary>
    public string Slug { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<ProjectMember> Members { get; set; } = [];
    public ICollection<TaskItem> Tasks { get; set; } = [];
}