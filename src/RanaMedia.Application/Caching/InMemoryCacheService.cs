using Microsoft.Extensions.Caching.Memory;

namespace RanaMedia.Application.Caching;

public interface IInMemoryCacheService
{
    Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    void Remove(string key);
}

public class InMemoryCacheService : IInMemoryCacheService
{
    private readonly IMemoryCache _cache;

    public InMemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (_cache.TryGetValue(key, out T? existing))
            return existing;

        var value = await factory();
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10),
            Size = 1
        };
        _cache.Set(key, value, options);
        return value;
    }

    public void Remove(string key) => _cache.Remove(key);
}

