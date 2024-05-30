using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Captivlink.Infrastructure.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetAsync<T>(string key, T value) where T : class
        {
            var json = JsonConvert.SerializeObject(value, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            var jsonAsBytes = Encoding.UTF8.GetBytes(json);
            await _cache.SetAsync(key, jsonAsBytes);
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            var jsonAsBytes = await _cache.GetAsync(key);
            if (jsonAsBytes == null) return null;

            var json = Encoding.UTF8.GetString(jsonAsBytes);
            var serializedObj = JsonConvert.DeserializeObject<T>(json);

            return serializedObj;
        }

        public async Task DeleteAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
