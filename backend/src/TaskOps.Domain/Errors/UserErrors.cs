using TaskOps.Domain.Common;

namespace TaskOps.Domain.Errors;

/// <summary>
/// Centralized error definitions for User domain operations.
/// Use these instead of creating Error instances inline in services.
/// </summary>
public static class UserErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("User.NotFound", $"User with ID '{id}' was not found.");

    public static Error EmailAlreadyExists(string email) =>
        Error.Conflict("User.EmailAlreadyExists", $"Email '{email}' is already registered.");

    public static Error InvalidCredentials() =>
        Error.Unauthorized("User.InvalidCredentials", "Email or password is incorrect.");

    public static Error EmailNotVerified() =>
        Error.Forbidden("User.EmailNotVerified", "Please verify your email before logging in.");
}