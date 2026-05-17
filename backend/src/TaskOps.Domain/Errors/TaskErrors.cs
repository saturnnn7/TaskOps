using TaskOps.Domain.Common;
using TaskOps.Domain.Enums;

namespace TaskOps.Domain.Errors;

public static class TaskErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Task.NotFound", $"Task with ID '{id}' was not found.");

    public static Error AccessDenied() =>
        Error.Forbidden("Task.AccessDenied", "You do not have permission to modify this task.");

    public static Error InvalidStatusTransition(WorkTaskStatus from, WorkTaskStatus to) =>
        Error.Conflict("Task.InvalidStatusTransition",
            $"Cannot transition task from '{from}' to '{to}'.");

    public static Error AssigneeNotMember(Guid userId) =>
        Error.Conflict("Task.AssigneeNotMember",
            $"User '{userId}' is not a member of this project and cannot be assigned.");
}