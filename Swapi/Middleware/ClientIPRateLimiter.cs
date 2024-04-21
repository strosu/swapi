using Microsoft.AspNetCore.RateLimiting;
using RedisRateLimiting;
using StackExchange.Redis;
using System.Threading.RateLimiting;

namespace Swapi.Middleware
{
    public class ClientIdRateLimiterPolicy(
        IConnectionMultiplexer connectionMultiplexer) : IRateLimiterPolicy<string>
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer;

        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get => RateLimitHeaderHelper.OnRejectedMethod; }

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            var clientId = httpContext.Request.Headers["X-ClientId"].ToString();

            //return RedisRateLimitPartition.GetSlidingWindowRateLimiter(clientId, key => new RedisSlidingWindowRateLimiterOptions
            //{
            //    ConnectionMultiplexerFactory = () => _connectionMultiplexer,
            //    PermitLimit = 1,
            //    Window = TimeSpan.FromSeconds(10)
            //}

            return RedisRateLimitPartition.GetFixedWindowRateLimiter(clientId, key => new RedisFixedWindowRateLimiterOptions
            {
                ConnectionMultiplexerFactory = () => _connectionMultiplexer,
                PermitLimit = 1,
                Window = TimeSpan.FromSeconds(3)
            }
            );
        }
    }
}