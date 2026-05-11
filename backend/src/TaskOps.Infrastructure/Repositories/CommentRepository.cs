using Microsoft.EntityFrameworkCore;
using TaskOps.Domain.Entities;
using TaskOps.Domain.Interfaces;
using TaskOps.Infrastructure.Persistence;

namespace TaskOps.Infrastructure.Repositories;

public sealed class CommentRepository : BaseRepository<Comment>, ICommentRepository
{
    public CommentRepository(AppDbContext context) : base(context) { }
    
    public async Task<(IReadOnlyList<Comment> Items, int TotalCount)> GetTaskCommentsAsync(
        Guid taskId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Where(c => c.TaskId == taskId && !c.IsDeleted)
            .Include(c => c.Author)
            .OrderBy(c => c.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}