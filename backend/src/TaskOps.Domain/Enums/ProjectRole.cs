namespace TaskOps.Domain.Enums;

/// <summary>
/// Defines a user's role within a specific project.
/// Roles control what actions a user can perform inside the project.
/// </summary>
public enum ProjectRole
{
    /// <summary>Read-only access to project and tasks.</summary>
    Viewer = 0,

    /// <summary>Can create and manage tasks, cannot delete project.</summary>
    Member = 1,

    /// <summary>Full control: edit/delete project, manage members.</summary>
    Owner = 2
}