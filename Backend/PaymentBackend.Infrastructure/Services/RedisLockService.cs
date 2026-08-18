using PaymentBackend.Application.Interfaces;
using StackExchange.Redis;

namespace PaymentBackend.Infrastructure.Services;

public class RedisLockService : IDistributedLockService
{
    private readonly IDatabase _database;

    public RedisLockService(IConnectionMultiplexer redisConnection)
    {
        _database = redisConnection.GetDatabase();
    }

    public async Task<bool> AcquireLockAsync(string resourceKey, string token, TimeSpan expiry)
    {
        return await _database.LockTakeAsync(resourceKey, token, expiry);
    }

    public async Task<bool> ReleaseLockAsync(string resourceKey, string token)
    {
        return await _database.LockReleaseAsync(resourceKey, token);
    }
}