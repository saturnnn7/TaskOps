using TaskOps.Application.Common.Models;

namespace TaskOps.Application.DTOs.Tasks;

/// <summary>
/// Query parameters for paginated task list with optional filters.
/// </summary>
public sealed class GetTasksQuery : PagedRequest
{
    /// <summary>Filter by task status. Example: "Todo", "InProgress"</summary>
    public string? Status { get; init; }

    /// <summary>Filter by assignee user ID.</summary>
    public Guid? AssigneeId { get; init; }
}