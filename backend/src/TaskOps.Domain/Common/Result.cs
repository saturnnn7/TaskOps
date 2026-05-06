namespace TaskOps.Domain.Common;

/// <summary>
/// Represents the outcome of an operation that can either succeed or fail.
/// Use this instead of throwing exceptions for expected business logic failures.
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public Error? Error { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = null;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Value = default;
        Error = error;
    }

    /// <summary>Creates a successful result with a value.</summary>
    public static Result<T> Success(T value) => new(value);
    
    /// <summary>Creates a failed result with an error.</summary>
    public static Result<T> Failure(Error error) => new(error);
}

/// <summary>
/// Represents the outcome of an operation that returns no value.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }

    private Result()
    {
        IsSuccess = true;
        Error = null;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    /// <summary>Creates a successful result with no value.</summary>
    public static Result Success() => new();

    /// <summary>Creates a failed result with an error.</summary>    
    public static Result Failure(Error error) => new(error);
}