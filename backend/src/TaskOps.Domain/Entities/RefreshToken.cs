using TaskOps.Domain.Common;

namespace TaskOps.Domain.Entities;

/// <summary>
/// Represents a refresh token stored in Redis.
/// Used to issue new access tokens without re-authentication.
/// Stored as a Redis key-value pair, not in PostgreSQL.
/// This class is used only for serialization/deserialization.
/// </summary>
public class RefreshToken
{
    public Guid UserId { get; init; }
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>IP address from which the token was issued.</summary>
    public string? CreatedByIp { get; init; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}