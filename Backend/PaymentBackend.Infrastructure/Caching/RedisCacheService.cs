using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using PaymentBackend.Application.Interfaces;

namespace PaymentBackend.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }
    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cachedData = await _cache.GetStringAsync(key, cancellationToken);
        
        if (string.IsNullOrEmpty(cachedData))
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(cachedData);
    }
    public async Task SetAsync<T>(string key, T data, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime ?? TimeSpan.FromMinutes(15)
        };
        var serializedData = JsonSerializer.Serialize(data);
        await _cache.SetStringAsync(key, serializedData, options, cancellationToken);
    }
}