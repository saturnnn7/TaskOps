using TaskOps.Domain.Entities;
using TaskOps.Domain.Enums;

namespace TaskOps.Domain.Interfaces;

/// <summary>
/// Task-specific repository operations.
/// </summary>
public interface ITaskRepository : IRepository<TaskItem>
{
    /// <summary>
    /// Returns a paginated list of tasks within a project,
    /// optionally filtered by status and/or assignee.
    /// </summary>
    Task<(IReadOnlyList<TaskItem> Items, int TotalCount)> GetProjectTasksAsync(
        Guid projectId,
        int page,
        int pageSize,
        WorkTaskStatus? status = null,
        Guid? assigneeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>Gets a task with its comments loaded.</summary>
    Task<TaskItem?> GetWithCommentsAsync(
        Guid taskId,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the maximum position value within a project and status column.</summary>
    Task<int> GetMaxPositionAsync(
        Guid projectId,
        WorkTaskStatus status,
        CancellationToken cancellationToken = default);
}