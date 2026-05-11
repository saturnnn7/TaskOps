using TaskOps.Application.Common.Interfaces;
using TaskOps.Application.DTOs.Auth;
using TaskOps.Domain.Common;
using TaskOps.Domain.Entities;
using TaskOps.Domain.Errors;
using TaskOps.Domain.Interfaces;

namespace TaskOps.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default)
    {
        // Check if email is already registered
        if (await _unitOfWork.Users.EmailExistsAsync(dto.Email, cancellationToken))
            return Result<AuthResponseDto>.Failure(
                UserErrors.EmailAlreadyExists(dto.Email));

        var user = new User
        {
            Email = dto.Email.ToLower().Trim(),
            DisplayName = dto.DisplayName.Trim(),
            PasswordHash = _passwordService.HashPassword(dto.Password)
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AuthResponseDto>.Success(await BuildAuthResponseAsync(
            user, ipAddress: null, cancellationToken));
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(
        LoginDto dto,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users
            .GetByEmailAsync(dto.Email, cancellationToken);

        // Use generic error — don't reveal whether email exists
        if (user is null || !_passwordService.VerifyPassword(dto.Password, user.PasswordHash))
            return Result<AuthResponseDto>.Failure(UserErrors.InvalidCredentials());

        // Update last login timestamp
        user.LastLoginAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AuthResponseDto>.Success(
            await BuildAuthResponseAsync(user, ipAddress, cancellationToken));
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var userId = await _tokenService.ValidateRefreshTokenAsync(
            refreshToken, cancellationToken);

        if (userId is null)
            return Result<AuthResponseDto>.Failure(
                UserErrors.InvalidCredentials());

        var user = await _unitOfWork.Users.GetByIdAsync(userId.Value, cancellationToken);

        if (user is null)
            return Result<AuthResponseDto>.Failure(
                UserErrors.NotFound(userId.Value));

        // Revoke the old refresh token — one-time use
        await _tokenService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);

        return Result<AuthResponseDto>.Success(
            await BuildAuthResponseAsync(user, ipAddress, cancellationToken));
    }

    public async Task<Result> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        await _tokenService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
        return Result.Success();
    }

    // Builds the auth response — generates both tokens and returns DTO
    private async Task<AuthResponseDto> BuildAuthResponseAsync(
        User user,
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        await _tokenService.SaveRefreshTokenAsync(
            user.Id, refreshToken, ipAddress, cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName
            }
        };
    }
}