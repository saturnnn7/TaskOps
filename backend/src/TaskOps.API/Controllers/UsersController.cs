using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskOps.Application.Common.Interfaces;
using TaskOps.Application.DTOs.Users;

namespace TaskOps.API.Controllers;

/// <summary>
/// Manages the current user's profile.
/// All endpoints require authentication.
/// </summary>
[Authorize]
public sealed class UsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<UpdateProfileDto> _updateProfileValidator;
    private readonly IValidator<ChangePasswordDto> _changePasswordValidator;

    public UsersController(
        IUserService userService,
        ICurrentUserService currentUser,
        IValidator<UpdateProfileDto> updateProfileValidator,
        IValidator<ChangePasswordDto> changePasswordValidator)
    {
        _userService = userService;
        _currentUser = currentUser;
        _updateProfileValidator = updateProfileValidator;
        _changePasswordValidator = changePasswordValidator;
    }

    /// <summary>Returns the profile of the currently authenticated user.</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var result = await _userService.GetProfileAsync(
            _currentUser.UserId, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Updates display name of the current user.</summary>
    [HttpPatch("me")]
    public async Task<IActionResult> UpdateMe(
        [FromBody] UpdateProfileDto dto,
        CancellationToken cancellationToken)
    {
        var error = await ValidateAsync(_updateProfileValidator, dto, cancellationToken);
        if (error is not null) return error;

        var result = await _userService.UpdateProfileAsync(
            _currentUser.UserId, dto, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Changes the password of the current user.</summary>
    [HttpPost("me/change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDto dto,
        CancellationToken cancellationToken)
    {
        var error = await ValidateAsync(_changePasswordValidator, dto, cancellationToken);
        if (error is not null) return error;

        var result = await _userService.ChangePasswordAsync(
            _currentUser.UserId, dto, cancellationToken);

        return HandleResult(result);
    }
}