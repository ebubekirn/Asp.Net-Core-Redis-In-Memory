using StackExchange.Redis;
using System.Text.Json;

namespace CachingWebAPI.Services
{
    public class CacheService : ICacheService
    {
        IDatabase _cacheDb;
        public CacheService()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _cacheDb = redis.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            var value = _cacheDb.StringGet(key); // redis teki key e karşılık gelen data gelir.
            if (!string.IsNullOrEmpty(value))
                return JsonSerializer.Deserialize<T>(value); // Redis ten aldığımız datayı deserialize etmeden return edemeyiz.

            return default;
        }

        public object RemoveData(string key)
        {
            var _exist = _cacheDb.KeyExists(key); // key varlığı kontrol edilip silme işlemi yapıldı.

            if (_exist)
                return _cacheDb.KeyDelete(key);

            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
        }
    }
}
