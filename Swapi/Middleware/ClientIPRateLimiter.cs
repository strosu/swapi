using Microsoft.AspNetCore.RateLimiting;
using RedisRateLimiting;
using StackExchange.Redis;
using System.Threading.RateLimiting;

namespace Swapi.Middleware
{
    public class ClientIdRateLimiterPolicy : IRateLimiterPolicy<string>
    {
        private readonly Func<OnRejectedContext, CancellationToken, ValueTask>? _onRejected;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public ClientIdRateLimiterPolicy(
            IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _onRejected = (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                return ValueTask.CompletedTask;
            };
        }

        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get => _onRejected; }

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            var clientId = httpContext.Request.Headers["X-ClientId"].ToString();

            return RedisRateLimitPartition.GetSlidingWindowRateLimiter(clientId, key => new RedisSlidingWindowRateLimiterOptions
            {
                ConnectionMultiplexerFactory = () => _connectionMultiplexer,
                PermitLimit = 1,
                Window = TimeSpan.FromSeconds(10)
        });
        }
    }
}
