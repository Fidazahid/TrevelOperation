namespace TravelOperation.Core.Interfaces;

/// <summary>
/// Service for caching frequently accessed data with TTL support
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached value by key, or creates and caches it using the factory function
    /// </summary>
    /// <typeparam name="T">Type of the cached value</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="factory">Function to create the value if not cached</param>
    /// <param name="absoluteExpirationMinutes">Time-to-live in minutes (default: 60)</param>
    /// <returns>The cached or newly created value</returns>
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, int absoluteExpirationMinutes = 60);

    /// <summary>
    /// Gets a cached value by key
    /// </summary>
    /// <typeparam name="T">Type of the cached value</typeparam>
    /// <param name="key">Cache key</param>
    /// <returns>The cached value, or default if not found</returns>
    T? Get<T>(string key);

    /// <summary>
    /// Sets a value in the cache with TTL
    /// </summary>
    /// <typeparam name="T">Type of the value</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="value">Value to cache</param>
    /// <param name="absoluteExpirationMinutes">Time-to-live in minutes (default: 60)</param>
    void Set<T>(string key, T value, int absoluteExpirationMinutes = 60);

    /// <summary>
    /// Removes a specific cache entry
    /// </summary>
    /// <param name="key">Cache key</param>
    void Remove(string key);

    /// <summary>
    /// Removes all cache entries matching a pattern
    /// </summary>
    /// <param name="pattern">Pattern to match (e.g., "lookup_*")</param>
    void RemoveByPattern(string pattern);

    /// <summary>
    /// Clears all cache entries
    /// </summary>
    void Clear();
}
