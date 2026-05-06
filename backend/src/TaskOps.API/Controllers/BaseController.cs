using Microsoft.AspNetCore.Mvc;
using TaskOps.Application.Common.Models;
using TaskOps.Domain.Common;

namespace TaskOps.API.Controllers;

/// <summary>
/// Base controller providing shared helper methods for mapping
/// Result[T] and domain errors to correct HTTP responses.
/// All API controllers must inherit from this class.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Maps a Result[T] to an appropriate HTTP response.
    /// Success → 200 OK with ApiResponse envelope.
    /// Failure → mapped HTTP status code with ApiError.
    /// </summary>
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(ApiResponse<T>.Ok(result.Value!));

        return HandleError(result.Error!);
    }

    /// <summary>
    /// Maps a Result (no value) to an appropriate HTTP response.
    /// Success → 204 No Content.
    /// Failure → mapped HTTP status code with ApiError.
    /// </summary>
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return NoContent();

        return HandleError(result.Error!);
    }

    /// <summary>
    /// Maps a Result[T] to 201 Created on success.
    /// Used for POST endpoints that create a new resource.
    /// </summary>
    protected IActionResult HandleCreated<T>(Result<T> result, string routeName, object routeValues)
    {
        if (result.IsSuccess)
            return CreatedAtRoute(routeName, routeValues, ApiResponse<T>.Ok(result.Value!));

        return HandleError(result.Error!);
    }

    /// <summary>
    /// Maps a domain Error to the correct HTTP status code and ApiResponse envelope.
    /// </summary>
    private ObjectResult HandleError(Error error)
    {
        var apiError = ApiError.From(error.Code, error.Message);

        return error.Type switch
        {
            ErrorType.NotFound     => NotFound(ApiResponse<object>.Fail(apiError)),
            ErrorType.Validation   => UnprocessableEntity(ApiResponse<object>.Fail(apiError)),
            ErrorType.Forbidden    => StatusCode(403, ApiResponse<object>.Fail(apiError)),
            ErrorType.Unauthorized => StatusCode(401, ApiResponse<object>.Fail(apiError)),
            ErrorType.Conflict     => Conflict(ApiResponse<object>.Fail(apiError)),
            ErrorType.Internal     => StatusCode(500, ApiResponse<object>.Fail(apiError)),
            _                      => StatusCode(500, ApiResponse<object>.Fail(apiError))
        };
    }
}