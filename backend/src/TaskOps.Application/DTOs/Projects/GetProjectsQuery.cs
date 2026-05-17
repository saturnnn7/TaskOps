using TaskOps.Application.Common.Models;

namespace TaskOps.Application.DTOs.Projects;

/// <summary>
/// Query parameters for paginated project list.
/// </summary>
public sealed class GetProjectsQuery : PagedRequest
{
    // Future: add search/filter fields here
}