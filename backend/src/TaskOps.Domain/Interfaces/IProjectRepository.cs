using TaskOps.Domain.Entities;
using TaskOps.Domain.Enums;

namespace TaskOps.Domain.Interfaces;

/// <summary>
/// Project-specific repository operations.
/// </summary>
public interface IProjectRepository : IRepository<Project>
{
    /// <summary>
    /// Returns a paginated list of projects where the user is a member.
    /// </summary>
    Task<(IReadOnlyList<Project> Items, int TotalCount)> GetUserProjectsAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>Finds a project by its unique slug.</summary>
    Task<Project?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>Checks if a slug is already taken.</summary>
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a project with its members loaded.
    /// Used for authorization checks.
    /// </summary>
    Task<Project?> GetWithMembersAsync(Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>Returns the role of a user within a project, or null if not a member.</summary>
    Task<ProjectRole?> GetUserRoleAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default);
}