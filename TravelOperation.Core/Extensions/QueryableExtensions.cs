using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Models;

namespace TravelOperation.Core.Extensions;

/// <summary>
/// Extension methods for IQueryable to add pagination support
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies pagination to a queryable and returns a paged result
    /// </summary>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PaginationParams pagination)
    {
        // Get total count before pagination
        var totalCount = await query.CountAsync();
        
        // Apply pagination
        var items = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync();
        
        return new PagedResult<T>(items, totalCount, pagination.PageNumber, pagination.PageSize);
    }

    /// <summary>
    /// Applies pagination to a queryable with custom page parameters
    /// </summary>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int pageNumber = 1,
        int pageSize = 50)
    {
        var pagination = new PaginationParams
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        
        return await query.ToPagedResultAsync(pagination);
    }

    /// <summary>
    /// Applies pagination and sorting to a queryable
    /// </summary>
    public static IQueryable<T> ApplyPagination<T>(
        this IQueryable<T> query,
        PaginationParams pagination)
    {
        return query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize);
    }

    /// <summary>
    /// Gets the total count and applies pagination in one operation (more efficient)
    /// </summary>
    public static async Task<(int TotalCount, List<T> Items)> GetPageAsync<T>(
        this IQueryable<T> query,
        int pageNumber = 1,
        int pageSize = 50)
    {
        var skip = (pageNumber - 1) * pageSize;
        
        // Execute both queries in parallel for better performance
        var countTask = query.CountAsync();
        var itemsTask = query.Skip(skip).Take(pageSize).ToListAsync();
        
        await Task.WhenAll(countTask, itemsTask);
        
        return (countTask.Result, itemsTask.Result);
    }
}
