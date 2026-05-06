using TaskOps.Domain.Common;

namespace TaskOps.Domain.Errors;

/// <summary>
/// Centralized error definitions for Project domain operations.
/// </summary>
public static class ProjectErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Project.NotFound", $"Project with ID '{id}' was not found.");

    public static Error SlugAlreadyExists(string slug) =>
        Error.Conflict("Project.SlugAlreadyExists", $"Project slug '{slug}' is already taken.");

    public static Error AccessDenied() =>
        Error.Forbidden("Project.AccessDenied", "You do not have permission to perform this action.");

    public static Error MemberAlreadyExists(Guid userId) =>
        Error.Conflict("Project.MemberAlreadyExists", $"User '{userId}' is already a member of this project.");

    public static Error OwnerCannotLeave() =>
        Error.Conflict("Project.OwnerCannotLeave", "Project owner cannot leave the project. Transfer ownership first.");
}