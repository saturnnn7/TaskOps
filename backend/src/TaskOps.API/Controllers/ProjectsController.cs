using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskOps.Application.Common.Interfaces;
using TaskOps.Application.Common.Models;
using TaskOps.Application.DTOs.Projects;

namespace TaskOps.API.Controllers;

/// <summary>
/// Manages projects and project membership.
/// All endpoints require authentication.
/// </summary>
[Authorize]
public sealed class ProjectsController : BaseController
{
    private readonly IProjectService _projectService;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateProjectDto> _createValidator;
    private readonly IValidator<UpdateProjectDto> _updateValidator;

    public ProjectsController(
        IProjectService projectService,
        ICurrentUserService currentUser,
        IValidator<CreateProjectDto> createValidator,
        IValidator<UpdateProjectDto> updateValidator)
    {
        _projectService = projectService;
        _currentUser = currentUser;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>Returns paginated list of projects the current user is a member of.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetProjectsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.GetUserProjectsAsync(
            _currentUser.UserId, query, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Returns a single project by ID.</summary>
    [HttpGet("{id:guid}", Name = "GetProjectById")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.GetByIdAsync(
            id, _currentUser.UserId, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Creates a new project. Current user becomes Owner.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectDto dto,
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

        var result = await _projectService.CreateAsync(
            _currentUser.UserId, dto, cancellationToken);

        return HandleCreated(result, "GetProjectById", new { id = result.Value?.Id });
    }

    /// <summary>Updates project name and/or description.</summary>
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProjectDto dto,
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

        var result = await _projectService.UpdateAsync(
            id, _currentUser.UserId, dto, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Archives the project. Only Owner can archive.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Archive(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.ArchiveAsync(
            id, _currentUser.UserId, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Returns all members of the project.</summary>
    [HttpGet("{id:guid}/members")]
    public async Task<IActionResult> GetMembers(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.GetMembersAsync(
            id, _currentUser.UserId, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Adds a user to the project. Only Owner can add members.</summary>
    [HttpPost("{id:guid}/members")]
    public async Task<IActionResult> AddMember(
        Guid id,
        [FromBody] AddMemberDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.AddMemberAsync(
            id, _currentUser.UserId, dto, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Removes a member from the project. Only Owner can remove members.</summary>
    [HttpDelete("{id:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.RemoveMemberAsync(
            id, _currentUser.UserId, userId, cancellationToken);

        return HandleResult(result);
    }
}