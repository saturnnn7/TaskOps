using TaskOps.Application.Common.Interfaces;
using TaskOps.Application.Common.Models;
using TaskOps.Application.DTOs.Tasks;
using TaskOps.Domain.Common;
using TaskOps.Domain.Entities;
using TaskOps.Domain.Enums;
using TaskOps.Domain.Errors;
using TaskOps.Domain.Interfaces;

namespace TaskOps.Application.Services;

public sealed class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;

    public TaskService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResponse<TaskDto>>> GetProjectTasksAsync(
        Guid projectId,
        Guid userId,
        GetTasksQuery query,
        CancellationToken cancellationToken = default)
    {
        // Verify user is a member of the project
        var role = await _unitOfWork.Projects
            .GetUserRoleAsync(projectId, userId, cancellationToken);

        if (role is null)
            return Result<PagedResponse<TaskDto>>.Failure(ProjectErrors.AccessDenied());

        // Parse optional status filter
        WorkTaskStatus? statusFilter = null;
        if (query.Status is not null &&
            Enum.TryParse<WorkTaskStatus>(query.Status, ignoreCase: true, out var parsedStatus))
            statusFilter = parsedStatus;

        var (items, totalCount) = await _unitOfWork.Tasks.GetProjectTasksAsync(
            projectId, query.Page, query.PageSize,
            statusFilter, query.AssigneeId, cancellationToken);

        var dtos = await MapToDtosAsync(items, cancellationToken);

        return Result<PagedResponse<TaskDto>>.Success(
            PagedResponse<TaskDto>.Create(dtos, query.Page, query.PageSize, totalCount));
    }

    public async Task<Result<TaskDto>> GetByIdAsync(
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken);

        if (task is null)
            return Result<TaskDto>.Failure(TaskErrors.NotFound(taskId));

        // Verify user is a member of the project
        var role = await _unitOfWork.Projects
            .GetUserRoleAsync(task.ProjectId, userId, cancellationToken);

        if (role is null)
            return Result<TaskDto>.Failure(ProjectErrors.AccessDenied());

        var dto = await MapToDtoAsync(task, cancellationToken);
        return Result<TaskDto>.Success(dto);
    }

    public async Task<Result<TaskDto>> CreateAsync(
        Guid projectId,
        Guid userId,
        CreateTaskDto dto,
        CancellationToken cancellationToken = default)
    {
        // Verify user is Owner or Member — Viewers cannot create tasks
        var role = await _unitOfWork.Projects
            .GetUserRoleAsync(projectId, userId, cancellationToken);

        if (role is null or ProjectRole.Viewer)
            return Result<TaskDto>.Failure(ProjectErrors.AccessDenied());

        // Verify assignee is a member of the project
        if (dto.AssigneeId.HasValue)
        {
            var assigneeRole = await _unitOfWork.Projects
                .GetUserRoleAsync(projectId, dto.AssigneeId.Value, cancellationToken);

            if (assigneeRole is null)
                return Result<TaskDto>.Failure(
                    TaskErrors.AssigneeNotMember(dto.AssigneeId.Value));
        }

        // Parse priority or default to None
        var priority = dto.Priority is not null &&
            Enum.TryParse<TaskPriority>(dto.Priority, ignoreCase: true, out var parsedPriority)
            ? parsedPriority
            : TaskPriority.None;

        // Position = max + 1 within the Todo column
        var maxPosition = await _unitOfWork.Tasks
            .GetMaxPositionAsync(projectId, WorkTaskStatus.Todo, cancellationToken);

        var task = new TaskItem
        {
            ProjectId = projectId,
            CreatedById = userId,
            AssigneeId = dto.AssigneeId,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            Status = WorkTaskStatus.Todo,
            Priority = priority,
            DueDate = dto.DueDate,
            Position = maxPosition + 1
        };

        await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var resultDto = await MapToDtoAsync(task, cancellationToken);
        return Result<TaskDto>.Success(resultDto);
    }

    public async Task<Result<TaskDto>> UpdateAsync(
        Guid taskId,
        Guid userId,
        UpdateTaskDto dto,
        CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken);

        if (task is null)
            return Result<TaskDto>.Failure(TaskErrors.NotFound(taskId));

        var role = await _unitOfWork.Projects
            .GetUserRoleAsync(task.ProjectId, userId, cancellationToken);

        // Only Owner and Member can update tasks
        if (role is null or ProjectRole.Viewer)
            return Result<TaskDto>.Failure(TaskErrors.AccessDenied());

        // Validate assignee is a project member
        if (dto.AssigneeId.HasValue)
        {
            var assigneeRole = await _unitOfWork.Projects
                .GetUserRoleAsync(task.ProjectId, dto.AssigneeId.Value, cancellationToken);

            if (assigneeRole is null)
                return Result<TaskDto>.Failure(
                    TaskErrors.AssigneeNotMember(dto.AssigneeId.Value));
        }

        if (dto.Title is not null)
            task.Title = dto.Title.Trim();

        if (dto.Description is not null)
            task.Description = dto.Description.Trim();

        if (dto.Status is not null &&
            Enum.TryParse<WorkTaskStatus>(dto.Status, ignoreCase: true, out var newStatus))
            task.Status = newStatus;

        if (dto.Priority is not null &&
            Enum.TryParse<TaskPriority>(dto.Priority, ignoreCase: true, out var newPriority))
            task.Priority = newPriority;

        if (dto.AssigneeId.HasValue)
            task.AssigneeId = dto.AssigneeId.Value;

        if (dto.DueDate.HasValue)
            task.DueDate = dto.DueDate.Value;

        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var resultDto = await MapToDtoAsync(task, cancellationToken);
        return Result<TaskDto>.Success(resultDto);
    }

    public async Task<Result> DeleteAsync(
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken);

        if (task is null)
            return Result.Failure(TaskErrors.NotFound(taskId));

        var role = await _unitOfWork.Projects
            .GetUserRoleAsync(task.ProjectId, userId, cancellationToken);

        // Owner can delete any task, Member can only delete their own tasks
        if (role is null or ProjectRole.Viewer)
            return Result.Failure(TaskErrors.AccessDenied());

        if (role == ProjectRole.Member && task.CreatedById != userId)
            return Result.Failure(TaskErrors.AccessDenied());

        _unitOfWork.Tasks.Delete(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    // Maps a list of TaskItem entities to TaskDtos
    private async Task<IReadOnlyList<TaskDto>> MapToDtosAsync(
        IReadOnlyList<TaskItem> tasks,
        CancellationToken cancellationToken)
    {
        var dtos = new List<TaskDto>();
        foreach (var task in tasks)
            dtos.Add(await MapToDtoAsync(task, cancellationToken));

        return dtos;
    }

    // Maps a single TaskItem entity to TaskDto
    private async Task<TaskDto> MapToDtoAsync(
        TaskItem task,
        CancellationToken cancellationToken)
    {
        User? assignee = null;
        if (task.AssigneeId.HasValue)
            assignee = await _unitOfWork.Users
                .GetByIdAsync(task.AssigneeId.Value, cancellationToken);

        var createdBy = await _unitOfWork.Users
            .GetByIdAsync(task.CreatedById, cancellationToken);

        var commentCount = await _unitOfWork.Comments
            .CountAsync(c => c.TaskId == task.Id && !c.IsDeleted, cancellationToken);

        return new TaskDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            AssigneeId = task.AssigneeId,
            AssigneeName = assignee?.DisplayName,
            CreatedById = task.CreatedById,
            CreatedByName = createdBy?.DisplayName ?? string.Empty,
            DueDate = task.DueDate,
            Position = task.Position,
            CommentCount = commentCount,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}