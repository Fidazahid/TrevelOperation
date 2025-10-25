using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using TravelOperation.Core.Interfaces;

namespace TravelOperation.Core.Services;

/// <summary>
/// Implementation of caching service using IMemoryCache
/// </summary>
public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, byte> _cacheKeys;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
        _cacheKeys = new ConcurrentDictionary<string, byte>();
    }

    /// <inheritdoc/>
    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, int absoluteExpirationMinutes = 60)
    {
        if (_cache.TryGetValue(key, out T? cachedValue) && cachedValue != null)
        {
            return cachedValue;
        }

        var value = await factory();
        Set(key, value, absoluteExpirationMinutes);
        return value;
    }

    /// <inheritdoc/>
    public T? Get<T>(string key)
    {
        _cache.TryGetValue(key, out T? value);
        return value;
    }

    /// <inheritdoc/>
    public void Set<T>(string key, T value, int absoluteExpirationMinutes = 60)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(absoluteExpirationMinutes))
            .RegisterPostEvictionCallback((k, v, r, s) =>
            {
                // Remove key from tracking when evicted
                _cacheKeys.TryRemove(k.ToString() ?? string.Empty, out _);
            });

        _cache.Set(key, value, cacheEntryOptions);
        _cacheKeys.TryAdd(key, 0);
    }

    /// <inheritdoc/>
    public void Remove(string key)
    {
        _cache.Remove(key);
        _cacheKeys.TryRemove(key, out _);
    }

    /// <inheritdoc/>
    public void RemoveByPattern(string pattern)
    {
        // Convert glob pattern to regex (simple * to .* conversion)
        var regexPattern = "^" + pattern.Replace("*", ".*") + "$";
        var regex = new System.Text.RegularExpressions.Regex(regexPattern);

        var keysToRemove = _cacheKeys.Keys.Where(k => regex.IsMatch(k)).ToList();
        
        foreach (var key in keysToRemove)
        {
            Remove(key);
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        var keysToRemove = _cacheKeys.Keys.ToList();
        
        foreach (var key in keysToRemove)
        {
            Remove(key);
        }
    }
}
