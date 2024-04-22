using Microsoft.AspNetCore.RateLimiting;
using RedisRateLimiting;
using StackExchange.Redis;
using System.Threading.RateLimiting;

namespace Swapi.Middleware.RateLimiter
{
    /// <summary>
    /// More permissive policy, for endpoints that don't consume too many resources
    /// </summary>
    /// <param name="connectionMultiplexer"></param>
    /// <param name="partitionStrategy"></param>
    public class PointQueryRateLimiterPolicy(
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

    /// <summary>
    /// More restrictive policy, for endpoints that consume more resources (i.e. they might result in several queries to the backplane)
    /// </summary>
    /// <param name="connectionMultiplexer"></param>
    /// <param name="partitionStrategy"></param>
    public class RangeQueryRateLimiterPolicy(
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