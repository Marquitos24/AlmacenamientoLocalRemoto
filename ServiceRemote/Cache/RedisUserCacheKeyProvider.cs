using ServiceRemote.Config;
using StackExchange.Redis;

namespace ServiceRemote.Cache;

public class RedisUserCacheKeyProvider : IUserCacheKeyProvider
{
    public IEnumerable<string> GetKeys(string pattern)
    {
        using var redis = ConnectionMultiplexer.Connect(ConfigRedis.ConnectionString);
        var server = redis.GetServer(ConfigRedis.ConnectionString);

        foreach (var key in server.Keys(pattern: pattern))
        {
            yield return key.ToString();
        }
    }
}