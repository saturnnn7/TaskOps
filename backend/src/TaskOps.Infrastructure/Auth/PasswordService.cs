using Isopoh.Cryptography.Argon2;
using TaskOps.Application.Common.Interfaces;

namespace TaskOps.Infrastructure.Auth;

/// <summary>
/// Argon2id password hashing implementation.
/// Argon2id is the recommended variant — resistant to both
/// side-channel attacks (Argon2i) and GPU attacks (Argon2d).
/// </summary>
public sealed class PasswordService : IPasswordService
{
    public string HashPassword(string password)
        => Argon2.Hash(password);

    public bool VerifyPassword(string password, string hash)
        => Argon2.Verify(hash, password);
}