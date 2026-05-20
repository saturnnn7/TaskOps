using TaskOps.Domain.Common;

namespace TaskOps.Domain.Errors;

public static class CommentErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Comment.NotFound", $"Comment with ID '{id}' was not found.");

    public static Error AccessDenied() =>
        Error.Forbidden("Comment.AccessDenied", "You do not have permission to modify this comment.");

    public static Error AlreadyDeleted() =>
        Error.Conflict("Comment.AlreadyDeleted", "This comment has already been deleted.");
}