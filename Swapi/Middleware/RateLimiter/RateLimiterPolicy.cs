using Microsoft.AspNetCore.RateLimiting;
using RedisRateLimiting;
using StackExchange.Redis;
using System.Threading.RateLimiting;

namespace Swapi.Middleware.RateLimiter
{
    /// <summary>
    /// Wires the request partitioning strategy with the actual PartitionGetter. This can be named during startup and applied to various controllers
    /// </summary>
    /// <param name="connectionMultiplexer"></param>
    /// <param name="partitionStrategy"></param>
    /// <param name="partitionGetter"></param>
    public class RateLimiterPolicy(
        IConnectionMultiplexer connectionMultiplexer,
        IPartitionStrategy partitionStrategy,
        IPartitionGetter partitionGetter) : IRateLimiterPolicy<string>
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer;
        private readonly IPartitionStrategy _partitionStrategy = partitionStrategy;
        private readonly IPartitionGetter _partitionGetter = partitionGetter;

        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get => RateLimitHeaderHelper.OnRejectedMethod; }

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            var partitionKey = _partitionStrategy.GetPartition();

            return _partitionGetter.GetPartition(partitionKey, _connectionMultiplexer);
        }
    }

    public interface IPartitionGetter 
    {
        RateLimitPartition<string> GetPartition(string partitionKey, IConnectionMultiplexer multiplexer);
    }

    public class SlidingWindowPartitionGetter : IPartitionGetter
    {
        public RateLimitPartition<string> GetPartition(string partitionKey, IConnectionMultiplexer multiplexer)
        {
            return RedisRateLimitPartition.GetSlidingWindowRateLimiter(partitionKey, key => new RedisSlidingWindowRateLimiterOptions
            {
                ConnectionMultiplexerFactory = () => multiplexer,
                PermitLimit = 2,
                Window = TimeSpan.FromSeconds(5)
            });
        }
    }
}