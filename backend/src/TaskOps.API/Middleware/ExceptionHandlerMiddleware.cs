using System.Net;
using System.Text.Json;
using TaskOps.Application.Common.Models;

namespace TaskOps.API.Middleware;

/// <summary>
/// Global exception handler middleware.
/// Catches all unhandled exceptions and returns a structured ApiResponse.
/// Prevents stack traces and internal details from leaking to clients.
/// </summary>
public sealed class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception on {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
            
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorCode, message) = exception switch
        {
            OperationCanceledException =>
                (HttpStatusCode.ServiceUnavailable, "Request.Cancelled", "Request was cancelled."),

            _ =>
                (HttpStatusCode.InternalServerError, "Server.Error", "An unexpected error occurred.")
        };

        var response = ApiResponse<object>.Fail(
            ApiError.From(errorCode, message));

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonOptions));
    }
}