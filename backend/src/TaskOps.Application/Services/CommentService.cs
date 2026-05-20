using TaskOps.Application.Common.Interfaces;
using TaskOps.Application.Common.Models;
using TaskOps.Application.DTOs.Comments;
using TaskOps.Domain.Common;
using TaskOps.Domain.Entities;
using TaskOps.Domain.Enums;
using TaskOps.Domain.Errors;
using TaskOps.Domain.Interfaces;

namespace TaskOps.Application.Services;

public sealed class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;

    public CommentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResponse<CommentDto>>> GetTaskCommentsAsync(
        Guid taskId,
        Guid userId,
        GetCommentsQuery query,
        CancellationToken cancellationToken = default)
    {
        // Verify task exists
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken);
        if (task is null)
            return Result<PagedResponse<CommentDto>>.Failure(TaskErrors.NotFound(taskId));

        // Verify user is a project member
        var role = await _unitOfWork.Projects
            .GetUserRoleAsync(task.ProjectId, userId, cancellationToken);

        if (role is null)
            return Result<PagedResponse<CommentDto>>.Failure(ProjectErrors.AccessDenied());

        var (items, totalCount) = await _unitOfWork.Comments
            .GetTaskCommentsAsync(taskId, query.Page, query.PageSize, cancellationToken);

        var dtos = items.Select(MapToDto).ToList();

        return Result<PagedResponse<CommentDto>>.Success(
            PagedResponse<CommentDto>.Create(dtos, query.Page, query.PageSize, totalCount));
    }

    public async Task<Result<CommentDto>> CreateAsync(
        Guid taskId,
        Guid userId,
        CreateCommentDto dto,
        CancellationToken cancellationToken = default)
    {
        // Verify task exists
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken);
        if (task is null)
            return Result<CommentDto>.Failure(TaskErrors.NotFound(taskId));

        // Verify user is a project member — all members can comment
        var role = await _unitOfWork.Projects
            .GetUserRoleAsync(task.ProjectId, userId, cancellationToken);

        if (role is null)
            return Result<CommentDto>.Failure(ProjectErrors.AccessDenied());

        var comment = new Comment
        {
            TaskId = taskId,
            AuthorId = userId,
            Content = dto.Content.Trim()
        };

        await _unitOfWork.Comments.AddAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Load author for response
        var author = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        comment.Author = author!;

        return Result<CommentDto>.Success(MapToDto(comment));
    }

    public async Task<Result<CommentDto>> UpdateAsync(
        Guid commentId,
        Guid userId,
        UpdateCommentDto dto,
        CancellationToken cancellationToken = default)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId, cancellationToken);

        if (comment is null)
            return Result<CommentDto>.Failure(CommentErrors.NotFound(commentId));

        if (comment.IsDeleted)
            return Result<CommentDto>.Failure(CommentErrors.AlreadyDeleted());

        // Only author can update their own comment
        if (comment.AuthorId != userId)
            return Result<CommentDto>.Failure(CommentErrors.AccessDenied());

        comment.Content = dto.Content.Trim();
        _unitOfWork.Comments.Update(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Load author for response
        var author = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        comment.Author = author!;

        return Result<CommentDto>.Success(MapToDto(comment));
    }

    public async Task<Result> DeleteAsync(
        Guid commentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId, cancellationToken);

        if (comment is null)
            return Result.Failure(CommentErrors.NotFound(commentId));

        if (comment.IsDeleted)
            return Result.Failure(CommentErrors.AlreadyDeleted());

        // Get the task to find the project
        var task = await _unitOfWork.Tasks.GetByIdAsync(comment.TaskId, cancellationToken);
        if (task is null)
            return Result.Failure(TaskErrors.NotFound(comment.TaskId));

        var role = await _unitOfWork.Projects
            .GetUserRoleAsync(task.ProjectId, userId, cancellationToken);

        // Author can delete their own comment, Owner can delete any comment
        var isAuthor = comment.AuthorId == userId;
        var isOwner = role == ProjectRole.Owner;

        if (!isAuthor && !isOwner)
            return Result.Failure(CommentErrors.AccessDenied());

        // Soft delete — preserve thread context
        comment.IsDeleted = true;
        _unitOfWork.Comments.Update(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    // Maps a Comment entity to CommentDto
    private static CommentDto MapToDto(Comment comment) => new()
    {
        Id = comment.Id,
        TaskId = comment.TaskId,
        AuthorId = comment.AuthorId,
        AuthorName = comment.Author?.DisplayName ?? string.Empty,
        Content = comment.IsDeleted ? "[deleted]" : comment.Content,
        IsDeleted = comment.IsDeleted,
        CreatedAt = comment.CreatedAt,
        UpdatedAt = comment.UpdatedAt
    };
}