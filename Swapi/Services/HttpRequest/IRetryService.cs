using Swapi.Services.HttpRequest;

namespace Swapi.Services.Http
{
    public interface IRetryService
    {
        Task Wait(HttpResponseMessage? result);

        void Reset();

        bool CanRetryFurther();
    }

    /// <summary>
    /// Service used as a strategy for waiting between failed requests
    /// Implements an exponential backoff; this works well when there's an outage, might not be the best for when throttled
    /// In addition to the 10k requests / day a single instance can send the backplane, the server seems to also throttled for a shorter time window(e.g. 15 mins)
    /// See https://github.com/semperry/swapi/blob/master/server/middleware/limiters.js for details
    /// </summary>
    public class ExponentialBackoffRetryService(ILogger<RequestService> logger) : IRetryService
    {
        private readonly ILogger<RequestService> _logger = logger;

        private static readonly int BackoffFactor = 2;

        private static readonly int MaxRetryCount = 10;

        /// <summary>
        /// Jitter to reduce the cluttering of requests
        /// </summary>
        private readonly int JitterMilliseconds = new Random().Next(100);

        private int _retryCount;

        /// <summary>
        /// Start with 1 second waiting time
        /// </summary>
        private int _millisecondsToWaitNext;

        public void Reset()
        {
            _retryCount = 0;
            _millisecondsToWaitNext = 1000;
        }

        public bool CanRetryFurther()
        {
            return _retryCount <= MaxRetryCount;
        }

        public async Task Wait(HttpResponseMessage? result)
        {
            if (_retryCount > MaxRetryCount)
            {
                throw new TimeoutException("Too many retries, giving up. Better luck next time.");
            }

            if (result?.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                throw new ProxyRateLimitedException("Too many requests. Please try again later.");
            }

            _retryCount++;

            var waitTime = _millisecondsToWaitNext + JitterMilliseconds;
            _logger.LogInformation($"Waiting for {waitTime} milliseconds");

            await Task.Delay(waitTime);

            _millisecondsToWaitNext *= BackoffFactor;
        }
    }
}
