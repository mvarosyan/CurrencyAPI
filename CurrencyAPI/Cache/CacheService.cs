using Microsoft.Extensions.Caching.Memory;

namespace CurrencyAPI.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T? Get<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out T value))
            {
                return value;
            }

            return default;
        }

        public void Set<T>(string key, T value, TimeSpan ttl)
        {
            _memoryCache.Set(key, value, ttl);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public void InvalidateAll()
        {
            _memoryCache.Dispose();
        }
    }
}
