using ZEN.Domain.Interfaces;

namespace ZEN.Infrastructure.Integrations.Redis;

/// <summary>
/// Null implementation of IRedisCache when Redis is not available
/// </summary>
public class NullRedisCache : IRedisCache
{
    public Task<string?> GetAsync(string key)
    {
        return Task.FromResult<string?>(null);
    }

    public Task RemoveAsync(string key)
    {
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix)
    {
        return Task.CompletedTask;
    }

    public Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        return Task.CompletedTask;
    }
}


