using System.Security.Claims;
using TaskOps.Application.Common.Interfaces;

namespace TaskOps.API.Services;

/// <summary>
/// Reads the current user's identity from the JWT claims.
/// Registered as Scoped — one instance per HTTP request.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier)
                ?? _httpContextAccessor.HttpContext?.User
                .FindFirstValue("sub");

            if (claim is null || !Guid.TryParse(claim, out var userId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            return userId;
        }
    }

    public bool IsAuthenticated
        => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}