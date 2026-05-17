using TaskOps.Application.Common.Models;
using TaskOps.Application.DTOs.Projects;
using TaskOps.Domain.Common;

namespace TaskOps.Application.Common.Interfaces;

/// <summary>
/// Handles all project-related business operations.
/// </summary>
public interface IProjectService
{
    /// <summary>Returns paginated list of projects the user is a member of.</summary>
    Task<Result<PagedResponse<ProjectDto>>> GetUserProjectsAsync(
        Guid userId,
        GetProjectsQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Returns a single project by ID if the user is a member.</summary>
    Task<Result<ProjectDto>> GetByIdAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>Creates a new project. The creator is automatically added as Owner.</summary>
    Task<Result<ProjectDto>> CreateAsync(
        Guid userId,
        CreateProjectDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>Updates project name and/or description. Only Owner or Member can update.</summary>
    Task<Result<ProjectDto>> UpdateAsync(
        Guid projectId,
        Guid userId,
        UpdateProjectDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>Archives a project. Only Owner can archive.</summary>
    Task<Result> ArchiveAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>Returns all members of a project.</summary>
    Task<Result<IReadOnlyList<ProjectMemberDto>>> GetMembersAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>Adds a user to the project. Only Owner can add members.</summary>
    Task<Result> AddMemberAsync(
        Guid projectId,
        Guid requestingUserId,
        AddMemberDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>Removes a member from the project. Owner cannot be removed.</summary>
    Task<Result> RemoveMemberAsync(
        Guid projectId,
        Guid requestingUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default);
}