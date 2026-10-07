namespace ServiceRemote.Cache;

public interface IUserCacheKeyProvider
{
    IEnumerable<string> GetKeys(string pattern);
}