using TaskOps.Application.Common.Helpers;
using TaskOps.Application.Common.Interfaces;
using TaskOps.Application.Common.Models;
using TaskOps.Application.DTOs.Projects;
using TaskOps.Domain.Common;
using TaskOps.Domain.Entities;
using TaskOps.Domain.Enums;
using TaskOps.Domain.Errors;
using TaskOps.Domain.Interfaces;

namespace TaskOps.Application.Services;

public sealed class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResponse<ProjectDto>>> GetUserProjectsAsync(
        Guid userId,
        GetProjectsQuery query,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _unitOfWork.Projects
            .GetUserProjectsAsync(userId, query.Page, query.PageSize, cancellationToken);

        // Load user roles for each project
        var dtos = new List<ProjectDto>();
        foreach (var project in items)
        {
            // Load project with members explicitly
            var projectWithMembers = await _unitOfWork.Projects
                .GetWithMembersAsync(project.Id, cancellationToken);
            
            if (projectWithMembers is null) continue;

            var role = projectWithMembers.Members
                .FirstOrDefault(m => m.UserId == userId)?.Role;

            dtos.Add(MapToDto(projectWithMembers, role, projectWithMembers.Members.Count));
        }

        return Result<PagedResponse<ProjectDto>>.Success(
            PagedResponse<ProjectDto>.Create(dtos, query.Page, query.PageSize, totalCount));
    }

    public async Task<Result<ProjectDto>> GetByIdAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var project = await _unitOfWork.Projects
            .GetWithMembersAsync(projectId, cancellationToken);

        if (project is null)
            return Result<ProjectDto>.Failure(ProjectErrors.NotFound(projectId));

        var role = project.Members
            .FirstOrDefault(m => m.UserId == userId)?.Role;

        if (role is null)
            return Result<ProjectDto>.Failure(ProjectErrors.AccessDenied());

        return Result<ProjectDto>.Success(
            MapToDto(project, role, project.Members.Count));
    }

    public async Task<Result<ProjectDto>> CreateAsync(
        Guid userId,
        CreateProjectDto dto,
        CancellationToken cancellationToken = default)
    {
        // Generate unique slug from project name
        var baseSlug = SlugHelper.Generate(dto.Name);
        var slug = await _unitOfWork.Projects.SlugExistsAsync(baseSlug, cancellationToken)
            ? SlugHelper.GenerateUnique(dto.Name)
            : baseSlug;

        var project = new Project
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            Slug = slug,
            Status = ProjectStatus.Active
        };

        await _unitOfWork.Projects.AddAsync(project, cancellationToken);

        // Creator is automatically the Owner
        var membership = new ProjectMember
        {
            ProjectId = project.Id,
            UserId = userId,
            Role = ProjectRole.Owner,
            JoinedAt = DateTime.UtcNow
        };

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Add membership after project is saved (FK constraint)
        await _unitOfWork.ProjectMembers.AddAsync(membership, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProjectDto>.Success(
            MapToDto(project, ProjectRole.Owner, 1));
    }

    public async Task<Result<ProjectDto>> UpdateAsync(
        Guid projectId,
        Guid userId,
        UpdateProjectDto dto,
        CancellationToken cancellationToken = default)
    {
        var project = await _unitOfWork.Projects
            .GetWithMembersAsync(projectId, cancellationToken);

        if (project is null)
            return Result<ProjectDto>.Failure(ProjectErrors.NotFound(projectId));

        var role = project.Members
            .FirstOrDefault(m => m.UserId == userId)?.Role;

        // Only Owner and Member can update
        if (role is null or ProjectRole.Viewer)
            return Result<ProjectDto>.Failure(ProjectErrors.AccessDenied());

        if (dto.Name is not null)
            project.Name = dto.Name.Trim();

        if (dto.Description is not null)
            project.Description = dto.Description.Trim();

        _unitOfWork.Projects.Update(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProjectDto>.Success(
            MapToDto(project, role.Value, project.Members.Count));
    }

    public async Task<Result> ArchiveAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var project = await _unitOfWork.Projects
            .GetWithMembersAsync(projectId, cancellationToken);

        if (project is null)
            return Result.Failure(ProjectErrors.NotFound(projectId));

        var role = project.Members
            .FirstOrDefault(m => m.UserId == userId)?.Role;

        // Only Owner can archive
        if (role != ProjectRole.Owner)
            return Result.Failure(ProjectErrors.AccessDenied());

        project.Status = ProjectStatus.Archived;
        _unitOfWork.Projects.Update(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IReadOnlyList<ProjectMemberDto>>> GetMembersAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var project = await _unitOfWork.Projects
            .GetWithMembersAsync(projectId, cancellationToken);

        if (project is null)
            return Result<IReadOnlyList<ProjectMemberDto>>.Failure(
                ProjectErrors.NotFound(projectId));

        // Any member can view the member list
        var isMember = project.Members.Any(m => m.UserId == userId);
        if (!isMember)
            return Result<IReadOnlyList<ProjectMemberDto>>.Failure(
                ProjectErrors.AccessDenied());

        // Load user details for each member
        var memberDtos = new List<ProjectMemberDto>();
        foreach (var member in project.Members)
        {
            var user = await _unitOfWork.Users
                .GetByIdAsync(member.UserId, cancellationToken);

            if (user is null) continue;

            memberDtos.Add(new ProjectMemberDto
            {
                UserId = member.UserId,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Role = member.Role.ToString(),
                JoinedAt = member.JoinedAt
            });
        }

        return Result<IReadOnlyList<ProjectMemberDto>>.Success(memberDtos);
    }

    public async Task<Result> AddMemberAsync(
        Guid projectId,
        Guid requestingUserId,
        AddMemberDto dto,
        CancellationToken cancellationToken = default)
    {
        var project = await _unitOfWork.Projects
            .GetWithMembersAsync(projectId, cancellationToken);

        if (project is null)
            return Result.Failure(ProjectErrors.NotFound(projectId));

        // Only Owner can add members
        var requestingRole = project.Members
            .FirstOrDefault(m => m.UserId == requestingUserId)?.Role;

        if (requestingRole != ProjectRole.Owner)
            return Result.Failure(ProjectErrors.AccessDenied());

        // Check if user exists
        var userToAdd = await _unitOfWork.Users
            .GetByIdAsync(dto.UserId, cancellationToken);

        if (userToAdd is null)
            return Result.Failure(UserErrors.NotFound(dto.UserId));

        // Check if already a member
        if (project.Members.Any(m => m.UserId == dto.UserId))
            return Result.Failure(ProjectErrors.MemberAlreadyExists(dto.UserId));

        // Parse role from string
        if (!Enum.TryParse<ProjectRole>(dto.Role, ignoreCase: true, out var role))
            role = ProjectRole.Member;

        var membership = new ProjectMember
        {
            ProjectId = projectId,
            UserId = dto.UserId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };

        project.Members.Add(membership);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RemoveMemberAsync(
        Guid projectId,
        Guid requestingUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        var project = await _unitOfWork.Projects
            .GetWithMembersAsync(projectId, cancellationToken);

        if (project is null)
            return Result.Failure(ProjectErrors.NotFound(projectId));

        var requestingRole = project.Members
            .FirstOrDefault(m => m.UserId == requestingUserId)?.Role;

        // Only Owner can remove members
        if (requestingRole != ProjectRole.Owner)
            return Result.Failure(ProjectErrors.AccessDenied());

        var targetMember = project.Members
            .FirstOrDefault(m => m.UserId == targetUserId);

        if (targetMember is null)
            return Result.Failure(UserErrors.NotFound(targetUserId));

        // Owner cannot be removed
        if (targetMember.Role == ProjectRole.Owner)
            return Result.Failure(ProjectErrors.OwnerCannotLeave());

        project.Members.Remove(targetMember);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    // Maps a Project entity to ProjectDto
    private static ProjectDto MapToDto(Project project, ProjectRole? role, int memberCount)
        => new()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Slug = project.Slug,
            Status = project.Status.ToString(),
            UserRole = role?.ToString() ?? string.Empty,
            MemberCount = memberCount,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
}