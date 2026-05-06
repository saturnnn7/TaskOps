namespace TaskOps.Application.Common.Models;

/// <summary>
/// Unified HTTP response envelope for all API endpoints.
/// All endpoints return this shape — frontend always knows what to expect.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public ApiError? Error { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    private ApiResponse() {}

    /// <summary>Creates a successful response with data.</summary>
    public static ApiResponse<T> Ok(T data) => new()
    {
        Success = true,
        Data = data
    };

    /// <summary>Creates a failed response with an error.</summary>
    public static ApiResponse<T> Fail(ApiError error) => new()
    {
        Success = false,
        Error = error
    };
}