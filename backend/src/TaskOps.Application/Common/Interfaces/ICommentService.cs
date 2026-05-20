using TaskOps.Application.Common.Models;
using TaskOps.Application.DTOs.Comments;
using TaskOps.Domain.Common;

namespace TaskOps.Application.Common.Interfaces;

/// <summary>
/// Handles all comment-related business operations.
/// All operations verify project membership before proceeding.
/// </summary>
public interface ICommentService
{
    /// <summary>Returns paginated comments for a task.</summary>
    Task<Result<PagedResponse<CommentDto>>> GetTaskCommentsAsync(
        Guid taskId,
        Guid userId,
        GetCommentsQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Creates a new comment on a task. User must be a project member.</summary>
    Task<Result<CommentDto>> CreateAsync(
        Guid taskId,
        Guid userId,
        CreateCommentDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>Updates comment content. Only the author can update.</summary>
    Task<Result<CommentDto>> UpdateAsync(
        Guid commentId,
        Guid userId,
        UpdateCommentDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-deletes a comment. Author can delete their own,
    /// Owner can delete any comment in their project.
    /// </summary>
    Task<Result> DeleteAsync(
        Guid commentId,
        Guid userId,
        CancellationToken cancellationToken = default);
}