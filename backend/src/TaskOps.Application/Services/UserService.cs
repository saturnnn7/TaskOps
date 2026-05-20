using TaskOps.Application.Common.Interfaces;
using TaskOps.Application.DTOs.Users;
using TaskOps.Domain.Common;
using TaskOps.Domain.Errors;
using TaskOps.Domain.Interfaces;

namespace TaskOps.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;

    public UserService(IUnitOfWork unitOfWork, IPasswordService passwordService)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
    }

    public async Task<Result<UserProfileDto>> GetProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);

        if (user is null) return Result<UserProfileDto>.Failure(UserErrors.NotFound(userId));

        return Result<UserProfileDto>.Success(MapToDto(user));
    }

    public async Task<Result<UserProfileDto>> UpdateProfileAsync(
        Guid userId,
        UpdateProfileDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);

        if (user is null) return Result<UserProfileDto>.Failure(UserErrors.NotFound(userId));

        if (dto.DisplayName is not null) user.DisplayName = dto.DisplayName.Trim();

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserProfileDto>.Success(MapToDto(user));
    }

    public async Task<Result> ChangePasswordAsync(
        Guid userId,
        ChangePasswordDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);

        if (user is null) return Result.Failure(UserErrors.NotFound(userId));

        // Verify current password before allowing change
        if (!_passwordService.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            return Result.Failure(UserErrors.WrongCurrentPassword());
        
        user.PasswordHash = _passwordService.HashPassword(dto.NewPassword);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    // Maps a User entity to UserProfileDto
    private static UserProfileDto MapToDto(Domain.Entities.User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName,
        IsEmailVerified = user.IsEmailVerified,
        LastLoginAt = user.LastLoginAt,
        CreatedAt = user.CreatedAt
    };
}