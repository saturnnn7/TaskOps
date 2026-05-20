using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskOps.Application.Common.Interfaces;
using TaskOps.Application.DTOs.Auth;
using TaskOps.Application.Common.Models;

namespace TaskOps.API.Controllers;

/// <summary>
/// Handles user registration, login, token refresh and logout.
/// All endpoints except Logout are anonymous.
/// </summary>
public sealed class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<LoginDto> _loginValidator;

    public AuthController(
        IAuthService authService,
        IValidator<RegisterDto> registerValidator,
        IValidator<LoginDto> loginValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    /// <summary>Registers a new user account.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDto dto,
        CancellationToken cancellationToken)
    {
        // Validate input before hitting the service
        var error = await ValidateAsync(_registerValidator, dto, cancellationToken);
        if (error is not null) return error;

        var result = await _authService.RegisterAsync(dto, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Authenticates a user and returns JWT tokens.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto dto,
        CancellationToken cancellationToken)
    {
        var error = await ValidateAsync(_loginValidator, dto, cancellationToken);
        if (error is not null) return error;

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.LoginAsync(dto, ipAddress, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Issues new access and refresh tokens using a valid refresh token.</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenDto dto,
        CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.RefreshTokenAsync(
            dto.RefreshToken, ipAddress, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>Invalidates the current refresh token (logout).</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshTokenDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LogoutAsync(dto.RefreshToken, cancellationToken);
        return HandleResult(result);
    }
}