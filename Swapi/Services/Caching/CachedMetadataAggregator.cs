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

        public Task<IEnumerable<T>> GetMetadataSetAsync<T>()
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetSingleMetadataAsync<T>(int objectId)
        {
            var result = await _database.StringGetAsync(GetRedisKey<T>(objectId));

            if (result != RedisValue.Null)
            {
                _logger.LogWarning($"Retrieved the result from Redis: {result}");
                return JsonSerializer.Deserialize<T>(result.ToString());
            }

            var remoteResult = await _httpMetadataAggregator.GetSingleMetadataAsync<T>(objectId);
            _logger.LogInformation("Persisting to redis");
            await _database.StringSetAsync(GetRedisKey<T>(objectId), JsonSerializer.Serialize(remoteResult), CacheDuration);

            return remoteResult;
        }

        private string GetRedisKey<T>(int? id = null)
        {
            if (id == null)
            {
                return $"{typeof(T).Name}";
            }

            return $"{typeof(T).Name}:{id}";
        }
    }
}
