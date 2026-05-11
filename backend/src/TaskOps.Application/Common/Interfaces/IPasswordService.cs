namespace TaskOps.Application.Common.Interfaces;

/// <summary>
/// Handles password hashing and verification using Argon2.
/// </summary>
public interface IPasswordService
{
    /// <summary>Hashes a plain text password using Argon2id.</summary>
    string HashPassword(string password);

    /// <summary>Verifies a plain text password against a stored Argon2 hash.</summary>
    bool VerifyPassword(string password, string hash);
}