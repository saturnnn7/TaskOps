namespace TaskOps.Application.Common.Models;

/// <summary>
/// Base class for all paginated query requests.
/// Inherit from this when a request needs pagination support.
/// </summary>
public abstract class PagedRequest
{
    private int _pageSize = 20;
    private int _page = 1;

    /// <summary>Page number, 1-based. Defaults to 1.</summary>
    public int Page
    {
        get => _page;
        init => _page = value < 1 ? 1 : value;
    }

    /// <summary>Items per page. Min 1, Max 100. Defaults to 20.</summary>
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value switch
        {
            < 1 => 1,
            > 100 => 100,
            _ => value
        };
    }
}