using Microsoft.AspNetCore.RateLimiting;
using RedisRateLimiting;
using StackExchange.Redis;
using System.Threading.RateLimiting;

namespace Swapi.Middleware.RateLimiter
{
    public class SingleRequestRateLimiterPolicy(
        IConnectionMultiplexer connectionMultiplexer,
        IPartitionStrategy partitionStrategy) : IRateLimiterPolicy<string>
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer;
        private readonly IPartitionStrategy _partitionStrategy = partitionStrategy;

        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get => RateLimitHeaderHelper.OnRejectedMethod; }

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            var partitionKey = _partitionStrategy.GetPartition();
            return RedisRateLimitPartition.GetSlidingWindowRateLimiter(partitionKey, key => new RedisSlidingWindowRateLimiterOptions
            {
                ConnectionMultiplexerFactory = () => _connectionMultiplexer,
                PermitLimit = 2,
                Window = TimeSpan.FromSeconds(5)
            });
        }
    }

    public class AggregateRequestRateLimiterPolicy(
    IConnectionMultiplexer connectionMultiplexer,
    IPartitionStrategy partitionStrategy) : IRateLimiterPolicy<string>
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer;
        private readonly IPartitionStrategy _partitionStrategy = partitionStrategy;

        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get => RateLimitHeaderHelper.OnRejectedMethod; }

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            var partitionKey = _partitionStrategy.GetPartition();
            return RedisRateLimitPartition.GetSlidingWindowRateLimiter(partitionKey, key => new RedisSlidingWindowRateLimiterOptions
            {
                ConnectionMultiplexerFactory = () => _connectionMultiplexer,
                PermitLimit = 1,
                Window = TimeSpan.FromSeconds(10)
            });
        }
    }
}