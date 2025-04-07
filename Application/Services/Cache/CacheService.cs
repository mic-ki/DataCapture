using Microsoft.Extensions.Caching.Memory;

namespace Application.Services.Cache;

public class CacheService(IMemoryCache memoryCache) : ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(memoryCache.TryGetValue(key, out T? value) ? value : default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        memoryCache.Set(key, value, expiration);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        // IMemoryCache nemá přímou metodu na vyčištění cache. 
        // Může být řešeno např. vytvořením nové instance IMemoryCache, pokud je to potřeba.
        return Task.CompletedTask;
    }
}
