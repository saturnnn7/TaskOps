using TaskOps.Application.Common.Models;
using TaskOps.Application.DTOs.Tasks;
using TaskOps.Domain.Common;

namespace TaskOps.Application.Common.Interfaces;

/// <summary>
/// Handles all task-related business operations.
/// All operations verify project membership before proceeding.
/// </summary>
public interface ITaskService
{
    /// <summary>Returns paginated tasks for a project with optional filters.</summary>
    Task<Result<PagedResponse<TaskDto>>> GetProjectTasksAsync(
        Guid projectId,
        Guid userId,
        GetTasksQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Returns a single task by ID with comment count.</summary>
    Task<Result<TaskDto>> GetByIdAsync(
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>Creates a new task in a project. User must be Owner or Member.</summary>
    Task<Result<TaskDto>> CreateAsync(
        Guid projectId,
        Guid userId,
        CreateTaskDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>Updates task fields. User must be Owner or Member.</summary>
    Task<Result<TaskDto>> UpdateAsync(
        Guid taskId,
        Guid userId,
        UpdateTaskDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>Deletes a task. Only Owner or task creator can delete.</summary>
    Task<Result> DeleteAsync(
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken = default);
}