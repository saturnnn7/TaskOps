using TaskOps.Domain.Common;

namespace TaskOps.Domain.Errors;

/// <summary>
/// Centralized error definitions for TaskItem domain operations.
/// </summary>
public static class TaskErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Task.NotFound", $"Task with ID '{id}' was not found.");

    public static Error AccessDenied() =>
        Error.Forbidden("Task.AccessDenied", "You do not have permission to modify this task.");

    public static Error InvalidStatusTransition(string from, string to) =>
        Error.Conflict("Task.InvalidStatusTransition", $"Cannot transition task from '{from}' to '{to}'.");
}