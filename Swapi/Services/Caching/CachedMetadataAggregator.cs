using StackExchange.Redis;
using System.Text.Json;

namespace Swapi.Services.Caching
{
    public class CachedMetadataAggregator(
        IMetadataAggregator httpMetadataAggregator, 
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<CachedMetadataAggregator> logger) : IMetadataAggregator
    {
        private readonly IMetadataAggregator _httpMetadataAggregator = httpMetadataAggregator;
        private readonly IDatabase _database = connectionMultiplexer.GetDatabase();
        private readonly ILogger<CachedMetadataAggregator> _logger = logger;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(10);

        public async Task<IEnumerable<T>> GetMetadataSetAsync<T>()
        {
            var key = GetRedisKey<T>();
            return await GetInternalAsync(key, _httpMetadataAggregator.GetMetadataSetAsync<T>);
        }

        public async Task<T> GetSingleMetadataAsync<T>(int objectId)
        {
            var key = GetRedisKey<T>(objectId);
            return await GetInternalAsync<T>(key, () => _httpMetadataAggregator.GetSingleMetadataAsync<T>(objectId));
        }

        private async Task<T> GetInternalAsync<T>(string key, Func<Task<T>> getDirectlyFunc)
        {
            var result = await _database.StringGetAsync(key);

            if (result != RedisValue.Null)
            {
                _logger.LogWarning($"Retrieved the result from Redis: {result}");
                return JsonSerializer.Deserialize<T>(result.ToString());
            }

            // If we couldn't find it in the cache, go retrieve it
            var remoteResult = await getDirectlyFunc();
            _logger.LogInformation("Persisting to redis");
            await _database.StringSetAsync(key, JsonSerializer.Serialize(remoteResult), CacheDuration);
            return remoteResult;
        }

        private static string GetRedisKey<T>(int? id = null)
        {
            if (id == null)
            {
                return $"{typeof(T).Name}";
            }

            return $"{typeof(T).Name}:{id}";
        }
    }
}
