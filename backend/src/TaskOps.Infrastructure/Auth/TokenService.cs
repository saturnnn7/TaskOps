using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskOps.Application.Common.Options;
using TaskOps.Application.Common.Interfaces;
using TaskOps.Domain.Entities;

namespace TaskOps.Infrastructure.Auth;

/// <summary>
/// RS256 JWT token service with Redis-backed refresh token storage.
/// Access tokens are signed with the private RSA key.
/// Refresh tokens are stored in Redis with TTL equal to expiration time.
/// </summary>
public sealed class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly IDistributedCache _cache;
    private readonly RsaSecurityKey _privateKey;
    private readonly RsaSecurityKey _publicKey;

    // Redis key prefixes
    private const string RefreshTokenPrefix = "refresh:";
    private const string UserTokensPrefix = "user_tokens:";

    public TokenService(IOptions<JwtOptions> jwtOptions, IDistributedCache cache)
    {
        _jwtOptions = jwtOptions.Value;
        _cache = cache;

        // Load RSA keys from PEM files
        var privateRsa = RSA.Create();
        privateRsa.ImportFromPem(File.ReadAllText(_jwtOptions.PrivateKeyPath));
        _privateKey = new RsaSecurityKey(privateRsa);

        var publicRsa = RSA.Create();
        publicRsa.ImportFromPem(File.ReadAllText(_jwtOptions.PublicKeyPath));
        _publicKey = new RsaSecurityKey(publicRsa);
    }

    public string GenerateAccessToken(User user)
    {
        var credentials = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.DisplayName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public async Task SaveRefreshTokenAsync(
        Guid userId,
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };

        var expiry = TimeSpan.FromDays(_jwtOptions.RefreshTokenExpirationDays);

        // Store token data: refresh:{token} → userId + metadata
        await _cache.SetStringAsync(
            $"{RefreshTokenPrefix}{refreshToken}",
            JsonSerializer.Serialize(data),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry },
            cancellationToken);

        // Track all tokens for this user: user_tokens:{userId} → list of tokens
        // Used for "logout from all devices"
        var userTokensKey = $"{UserTokensPrefix}{userId}";
        var existingJson = await _cache.GetStringAsync(userTokensKey, cancellationToken);

        var tokens = existingJson is not null
            ? JsonSerializer.Deserialize<List<string>>(existingJson) ?? []
            : new List<string>();

        tokens.Add(refreshToken);

        await _cache.SetStringAsync(
            userTokensKey,
            JsonSerializer.Serialize(tokens),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry },
            cancellationToken);
    }

    public async Task<Guid?> ValidateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var json = await _cache.GetStringAsync(
            $"{RefreshTokenPrefix}{refreshToken}",
            cancellationToken);

        if (json is null) return null;

        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var userIdString = data.GetProperty("UserId").GetString();

        return Guid.TryParse(userIdString, out var userId) ? userId : null;
    }

    public async Task RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
        => await _cache.RemoveAsync(
            $"{RefreshTokenPrefix}{refreshToken}",
            cancellationToken);

    public async Task RevokeAllUserTokensAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userTokensKey = $"{UserTokensPrefix}{userId}";
        var json = await _cache.GetStringAsync(userTokensKey, cancellationToken);

        if (json is null) return;

        var tokens = JsonSerializer.Deserialize<List<string>>(json) ?? [];

        // Remove each refresh token
        foreach (var token in tokens)
            await _cache.RemoveAsync($"{RefreshTokenPrefix}{token}", cancellationToken);

        // Remove the user tokens index
        await _cache.RemoveAsync(userTokensKey, cancellationToken);
    }

    /// <summary>
    /// Returns the public RSA key for JWT validation.
    /// Used by ServiceCollectionExtensions to configure JwtBearer.
    /// </summary>
    public RsaSecurityKey GetPublicKey() => _publicKey;
}