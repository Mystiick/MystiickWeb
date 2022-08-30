namespace MystiickWeb.Shared.Services;

[Injectable(typeof(CacheService), InjectableAttribute.InjectableSetting.Singleton)]
public class CacheService
{
    private readonly Dictionary<string, object> _cache = new();

    public void Save<T>(string key, T value) where T : notnull
    {
        if (_cache.ContainsKey(key))
        {
            _cache[key] = value;
        }
        else
        {
            _cache.Add(key, value);
        }
    }

    public T Load<T>(string key)
    {
        return (T)_cache[key];
    }

    public bool Delete(string key)
    {
        if (_cache.ContainsKey(key))
        {
            _cache.Remove(key);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Contains(string key)
    {
        return _cache.ContainsKey(key);
    }
}
