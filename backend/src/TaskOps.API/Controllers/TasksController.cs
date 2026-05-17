using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskOps.Application.Common.Interfaces;
using TaskOps.Application.Common.Models;
using TaskOps.Application.DTOs.Tasks;

namespace TaskOps.API.Controllers;

/// <summary>
/// Manages tasks within a project.
/// All endpoints require authentication and project membership.
/// </summary>
[Authorize]
public sealed class TasksController : BaseController
{
    private readonly ITaskService _taskService;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateTaskDto> _createValidator;
    private readonly IValidator<UpdateTaskDto> _updateValidator;

    public TasksController(
        ITaskService taskService,
        ICurrentUserService currentUser,
        IValidator<CreateTaskDto> createValidator,
        IValidator<UpdateTaskDto> updateValidator)
    {
        _taskService = taskService;
        _currentUser = currentUser;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>Returns paginated tasks for a project with optional filters.</summary>
    [HttpGet("projects/{projectId:guid}/tasks")]
    public async Task<IActionResult> GetAll(
        Guid projectId,
        [FromQuery] GetTasksQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _taskService.GetProjectTasksAsync(
            projectId, _currentUser.UserId, query, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Returns a single task by ID.</summary>
    [HttpGet("tasks/{taskId:guid}", Name = "GetTaskById")]
    public async Task<IActionResult> GetById(
        Guid taskId,
        CancellationToken cancellationToken)
    {
        var result = await _taskService.GetByIdAsync(
            taskId, _currentUser.UserId, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Creates a new task in a project.</summary>
    [HttpPost("projects/{projectId:guid}/tasks")]
    public async Task<IActionResult> Create(
        Guid projectId,
        [FromBody] CreateTaskDto dto,
        CancellationToken cancellationToken)
    {
        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            var details = validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return UnprocessableEntity(ApiResponse<object>.Fail(
                ApiError.From("Validation.Failed", "One or more validation errors occurred.", details)));
        }

        var result = await _taskService.CreateAsync(
            projectId, _currentUser.UserId, dto, cancellationToken);

        return HandleCreated(result, "GetTaskById", new { taskId = result.Value?.Id });
    }

    /// <summary>Updates task fields.</summary>
    [HttpPatch("tasks/{taskId:guid}")]
    public async Task<IActionResult> Update(
        Guid taskId,
        [FromBody] UpdateTaskDto dto,
        CancellationToken cancellationToken)
    {
        var validation = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            var details = validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return UnprocessableEntity(ApiResponse<object>.Fail(
                ApiError.From("Validation.Failed", "One or more validation errors occurred.", details)));
        }

        var result = await _taskService.UpdateAsync(
            taskId, _currentUser.UserId, dto, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Deletes a task. Owner can delete any task, Member only their own.</summary>
    [HttpDelete("tasks/{taskId:guid}")]
    public async Task<IActionResult> Delete(
        Guid taskId,
        CancellationToken cancellationToken)
    {
        var result = await _taskService.DeleteAsync(
            taskId, _currentUser.UserId, cancellationToken);

        return HandleResult(result);
    }
}