using Microsoft.EntityFrameworkCore;
using TaskOps.Domain.Entities;
using TaskOps.Domain.Enums;
using TaskOps.Domain.Interfaces;
using TaskOps.Infrastructure.Persistence;

namespace TaskOps.Infrastructure.Repositories;

public sealed class TaskRepository : BaseRepository<TaskItem>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context) { }

    public async Task<(IReadOnlyList<TaskItem> Items, int TotalCount)> GetProjectTasksAsync(
        Guid projectId,
        int page,
        int pageSize,
        WorkTaskStatus? status = null,
        Guid? assigneeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Where(t => t.ProjectId == projectId)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (assigneeId.HasValue)
            query = query.Where(t => t.AssignedId == assigneeId.Value);

        query = query.OrderBy(t => t.Position).ThenByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<TaskItem?> GetWithCommentsAsync(
        Guid taskId,
        CancellationToken cancellationToken = default)
        => await DbSet
            .Include(t => t.Comments.Where(c => !c.IsDeleted))
            .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(c => c.Id == taskId, cancellationToken);

    public async Task<int> GetMaxPositionAsync(
        Guid projectId,
        WorkTaskStatus status,
        CancellationToken cancellationToken = default)
    {
        var max = await DbSet
            .Where(t => t.ProjectId == projectId && t.Status == status)
            .MaxAsync(t => (int?)t.Position, cancellationToken);

        return max ?? 0;
    }
}