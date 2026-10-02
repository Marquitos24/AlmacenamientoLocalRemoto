using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace ServiceRemote.Cache.Redis;
using StackExchange.Redis;

public class RedisCacheUserService(IConnectionMultiplexer connection) : IDistributedCache
{
    /// <inheritdoc/>
    public Task<byte[]?> GetAsync(string key)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public Task RemoveAsync(string key)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public Task RemoveAllAsync()
    {
        throw new System.NotImplementedException();
    }
}