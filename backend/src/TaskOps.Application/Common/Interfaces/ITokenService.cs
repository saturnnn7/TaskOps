using TaskOps.Domain.Entities;

namespace TaskOps.Application.Common.Interfaces;

/// <summary>
/// Handles JWT access token generation and refresh token management.
/// </summary>
public interface ITokenService
{
    /// <summary>Generates a signed RS256 JWT access token for the given user.</summary>
    string GenerateAccessToken(User user);

    /// <summary>Generates a cryptographically secure refresh token string.</summary>
    string GenerateRefreshToken();

    /// <summary>Saves a refresh token to Redis with TTL.</summary>
    Task SaveRefreshTokenAsync(
        Guid userId,
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates a refresh token from Redis.
    /// Returns the associated UserId if valid, null if expired or not found.
    /// </summary>
    Task<Guid?> ValidateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);
    
    /// <summary>Removes a refresh token from Redis (logout).</summary>
    Task RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);
    
    /// <summary>Removes all refresh tokens for a user (logout all devices).</summary>
    Task RevokeAllUserTokensAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}