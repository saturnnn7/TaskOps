using TaskOps.Application.Common.Models;

namespace TaskOps.Application.DTOs.Comments;

/// <summary>
/// Query parameters for paginated comment list.
/// </summary>
public sealed class GetCommentsQuery : PagedRequest { }