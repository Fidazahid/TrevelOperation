namespace TravelOperation.Core.Models;

/// <summary>
/// Generic paged result model for efficient pagination
/// </summary>
/// <typeparam name="T">The type of items in the result</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// The items for the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();
    
    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    
    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
    
    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
    
    /// <summary>
    /// First item number on current page (for display)
    /// </summary>
    public int FirstItemOnPage => (PageNumber - 1) * PageSize + 1;
    
    /// <summary>
    /// Last item number on current page (for display)
    /// </summary>
    public int LastItemOnPage => Math.Min(PageNumber * PageSize, TotalCount);

    /// <summary>
    /// Creates a new paged result
    /// </summary>
    public PagedResult()
    {
    }

    /// <summary>
    /// Creates a new paged result with items and metadata
    /// </summary>
    public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

/// <summary>
/// Pagination parameters for queries
/// </summary>
public class PaginationParams
{
    private int _pageNumber = 1;
    private int _pageSize = 50;
    
    /// <summary>
    /// Current page number (1-based). Defaults to 1.
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }
    
    /// <summary>
    /// Number of items per page. Defaults to 50. Max 1000.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            < 1 => 50,
            > 1000 => 1000,
            _ => value
        };
    }
    
    /// <summary>
    /// Sort column name
    /// </summary>
    public string? SortBy { get; set; }
    
    /// <summary>
    /// Sort direction (asc or desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";
    
    /// <summary>
    /// Whether to sort in descending order
    /// </summary>
    public bool IsDescending => SortDirection?.ToLower() == "desc";

    /// <summary>
    /// Calculate skip count for database query
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;
}
