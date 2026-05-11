using TaskOps.Application.DTOs.Auth;
using TaskOps.Domain.Common;

namespace TaskOps.Application.Common.Interfaces;

/// <summary>
/// Handles user registration, login and token refresh operations.
/// </summary>
public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<AuthResponseDto>> LoginAsync(
        LoginDto dto,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<Result<AuthResponseDto>> RefreshTokenAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<Result> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);
}