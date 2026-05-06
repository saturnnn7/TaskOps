namespace TaskOps.Application.Common.Models;

/// <summary>
/// Structured error object returned in all failed API responses.
/// </summary>
public class ApiError
{
    public string Code { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    
    /// <summary>Optional field-level validation errors.</summary>
    public Dictionary<string, string[]>? Details { get; init; }

    public static ApiError From(string code, string message, Dictionary<string, string[]>? details = null)
        => new() { Code = code, Message = message, Details = details };
}