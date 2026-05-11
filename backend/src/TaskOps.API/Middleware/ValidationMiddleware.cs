using FluentValidation;
using TaskOps.Application.Common.Models;
using System.Text.Json;

namespace TaskOps.API.Middleware;

/// <summary>
/// Catches FluentValidation exceptions thrown by validators
/// and maps them to structured ApiResponse with field-level errors.
/// </summary>
public sealed class ValidationMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var details = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());
            
            var response = ApiResponse<object>.Fail(
                ApiError.From(
                    "Validation.Failed",
                    "One or more validation errors occurred.",
                    details));
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 422;

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
        }
    }
}