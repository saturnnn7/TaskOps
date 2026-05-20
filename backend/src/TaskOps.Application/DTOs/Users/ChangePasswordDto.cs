namespace TaskOps.Application.DTOs.Users;

public sealed class ChangePasswordDto
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}