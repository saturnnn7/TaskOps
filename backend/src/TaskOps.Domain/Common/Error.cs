namespace TaskOps.Domain.Common;

/// <summary>
/// Represents a structured error with a code and human-readable message.
/// ErrorCode format: "Entity.Reason" e.g. "User.NotFound", "Task.AccessDenied"
/// </summary>
public sealed class Error
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }

    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    // --- Factories ---

    /// <summary>Resource was not found.</summary>
    public static Error NotFound(string code, string message)
        => new(code, message, ErrorType.NotFound);
    
    /// <summary>Input validation failed.</summary>
    public static Error Validation(string code, string message)
        => new(code, message, ErrorType.Validation);
    
    /// <summary>Action is not allowed for this user.</summary>
    public static Error Forbidden(string code, string message)
        => new(code, message, ErrorType.Forbidden);
    
    /// <summary>User is not authenticated.</summary>
    public static Error Unauthorized(string code, string message)
        => new(code, message, ErrorType.Unauthorized);
    
    /// <summary>Business rule was violated.</summary>
    public static Error Conflict(string code, string message)
        => new(code, message, ErrorType.Conflict);
    
    /// <summary>Unexpected internal error.</summary>
    public static Error Internal(string code, string message)
        => new(code, message, ErrorType.Internal);

    public override string ToString() => $"{Code}: {Message}";
}

/// <summary>
/// Defines the category of an error, used to map to HTTP status codes.
/// </summary>
public enum ErrorType
{
    NotFound = 0,
    Validation = 1,
    Forbidden = 2,
    Unauthorized = 3,
    Conflict = 4,
    Internal = 5
}