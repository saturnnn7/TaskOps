using Microsoft.EntityFrameworkCore;
using TaskOps.Domain.Entities;
using TaskOps.Domain.Enums;
using TaskOps.Domain.Interfaces;
using TaskOps.Infrastructure.Persistence;

namespace TaskOps.Infrastructure.Repositories;

public sealed class ProjectRepository : BaseRepository<Project>, IProjectRepository
{
    public ProjectRepository(AppDbContext context) : base(context) {}

    public async Task<(IReadOnlyList<Project> Items, int TotalCount)> GetUserProjectsAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.ProjectMembers
            .Where(pm => pm.UserId == userId)
            .Select(pm => pm.Project)
            .OrderByDescending(p => p.UpdatedAt);
        
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return (items, totalCount);
    }

    public async Task<Project?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        => await DbSet.FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(p => p.Slug == slug, cancellationToken);

    public async Task<Project?> GetWithMembersAsync(Guid projectId, CancellationToken cancellationToken = default)
        => await DbSet
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

    public async Task<ProjectRole?> GetUserRoleAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var member = await Context.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId, cancellationToken);
        
        return member?.Role;
    }
}