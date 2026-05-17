namespace TaskOps.Application.Common.Interfaces;

/// <summary>
/// Provides access to the currently authenticated user's identity.
/// Extracts claims from the JWT token on each request.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Returns the ID of the currently authenticated user.
    /// Throws if called outside of an authenticated request.
    /// </summary>
    Guid UserId { get; }

    /// <summary>Returns true if the current request is authenticated.</summary>
    bool IsAuthenticated { get; }
}