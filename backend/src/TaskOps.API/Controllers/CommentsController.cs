using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskOps.Application.Common.Interfaces;
using TaskOps.Application.Common.Models;
using TaskOps.Application.DTOs.Comments;

namespace TaskOps.API.Controllers;

/// <summary>
/// Manages comments on tasks.
/// All endpoints require authentication and project membership.
/// </summary>
[Authorize]
public sealed class CommentsController : BaseController
{
    private readonly ICommentService _commentService;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateCommentDto> _createValidator;
    private readonly IValidator<UpdateCommentDto> _updateValidator;

    public CommentsController(
        ICommentService commentService,
        ICurrentUserService currentUser,
        IValidator<CreateCommentDto> createValidator,
        IValidator<UpdateCommentDto> updateValidator)
    {
        _commentService = commentService;
        _currentUser = currentUser;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>Returns paginated comments for a task.</summary>
    [HttpGet("tasks/{taskId:guid}/comments")]
    public async Task<IActionResult> GetAll(
        Guid taskId,
        [FromQuery] GetCommentsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _commentService.GetTaskCommentsAsync(
            taskId, _currentUser.UserId, query, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Creates a new comment on a task.</summary>
    [HttpPost("tasks/{taskId:guid}/comments")]
    public async Task<IActionResult> Create(
        Guid taskId,
        [FromBody] CreateCommentDto dto,
        CancellationToken cancellationToken)
    {
        var error = await ValidateAsync(_createValidator, dto, cancellationToken);
        if (error is not null) return error;

        var result = await _commentService.CreateAsync(
            taskId, _currentUser.UserId, dto, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Updates comment content. Only the author can update.</summary>
    [HttpPatch("comments/{commentId:guid}")]
    public async Task<IActionResult> Update(
        Guid commentId,
        [FromBody] UpdateCommentDto dto,
        CancellationToken cancellationToken)
    {
        var error = await ValidateAsync(_updateValidator, dto, cancellationToken);
        if (error is not null) return error;

        var result = await _commentService.UpdateAsync(
            commentId, _currentUser.UserId, dto, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Soft-deletes a comment.</summary>
    [HttpDelete("comments/{commentId:guid}")]
    public async Task<IActionResult> Delete(
        Guid commentId,
        CancellationToken cancellationToken)
    {
        var result = await _commentService.DeleteAsync(
            commentId, _currentUser.UserId, cancellationToken);

        return HandleResult(result);
    }
}