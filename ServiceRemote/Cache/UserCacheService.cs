using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ServiceRemote.Models;

namespace ServiceRemote.Cache;

public class UserCacheService() : IUserCache
{
    public UserCacheService(Microsoft.Extensions.Caching.Distributed.IDistributedCache cache, IUserCacheKeyProvider? keyProvider = null) : this()
    {
        _cache = cache;
        _keyProvider = keyProvider ?? new RedisUserCacheKeyProvider();
    }
 
    private Microsoft.Extensions.Caching.Distributed.IDistributedCache _cache;

    private readonly IUserCacheKeyProvider _keyProvider;

    /// <inheritdoc/>
    public async Task<User?> GetAsync(string id)
    {
        var json = await _cache.GetAsync($"user:{id}");
        return json is null ? null : JsonSerializer.Deserialize<User>(json);
    }

    /// <inheritdoc/>
    public async Task SetAsync(User user)
    {
        string json = JsonSerializer.Serialize(user);
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(user));
        await _cache.SetAsync(
            $"user:{user.Id}",
            bytes,
            new DistributedCacheEntryOptions());
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync($"user:{key}");
    }

    /// <inheritdoc/>
    public async Task RemoveAllAsync()
    {
        foreach (var key in _keyProvider.GetKeys("user:*"))
        {
            await _cache.RemoveAsync(key);
        }
    }
}
