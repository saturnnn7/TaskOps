using TaskOps.Domain.Entities;

namespace TaskOps.Domain.Interfaces;

/// <summary>
/// Comment-specific repository operations.
/// </summary>
public interface ICommentRepository : IRepository<Comment>
{
    /// <summary>
    /// Returns paginated comments for a task, excluding soft-deleted ones.
    /// </summary>
    Task<(IReadOnlyList<Comment> Items, int TotalCount)> GetTaskCommentsAsync(
        Guid taskId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}