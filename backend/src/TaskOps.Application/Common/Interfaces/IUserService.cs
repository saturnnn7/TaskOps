using TaskOps.Application.DTOs.Users;
using TaskOps.Domain.Common;

namespace TaskOps.Application.Common.Interfaces;

/// <summary>
/// Handles user profile operations.
/// </summary>
public interface IUserService
{
    /// <summary>Returns the profile of the currently authenticated user.</summary>
    Task<Result<UserProfileDto>> GetProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>Updates display name of the current user.</summary>
    Task<Result<UserProfileDto>> UpdateProfileAsync(
        Guid userId,
        UpdateProfileDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>Changes the password of the current user.</summary>
    Task<Result> ChangePasswordAsync(
        Guid userId,
        ChangePasswordDto dto,
        CancellationToken cancellationToken = default);
}