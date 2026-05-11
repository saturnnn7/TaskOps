namespace TaskOps.Application.DTOs.Auth;

public sealed class RegisterDto
{
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}