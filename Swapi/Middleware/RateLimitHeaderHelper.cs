using Microsoft.AspNetCore.RateLimiting;
using RedisRateLimiting;
using RedisRateLimiting.AspNetCore;

namespace Swapi.Middleware
{
    internal static class RateLimitHeaderHelper
    {

        public static Func<OnRejectedContext, CancellationToken, ValueTask> OnRejectedMethod =
            async (rejectedContext, token) =>
        {
            var lease = rejectedContext.Lease;
            var httpContext = rejectedContext.HttpContext;

            httpContext.Response.StatusCode = 429;

            if (lease.TryGetMetadata(RateLimitMetadataName.Limit, out var limit))
            {
                httpContext.Response.Headers[RateLimitHeaders.Limit] = limit;
            }

            if (lease.TryGetMetadata(RateLimitMetadataName.Remaining, out var remaining))
            {
                httpContext.Response.Headers[RateLimitHeaders.Remaining] = remaining.ToString();
            }

            if (lease.TryGetMetadata(RateLimitMetadataName.RetryAfter, out var retryAfter))
            {
                httpContext.Response.Headers[RateLimitHeaders.RetryAfter] = retryAfter.ToString();
            }

            await httpContext.Response.WriteAsync("Too many requests. Please try again later.");
        };
    }
}