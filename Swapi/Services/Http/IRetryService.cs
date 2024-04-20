using Microsoft.Extensions.Logging;

namespace Swapi.Services.Http
{
    public interface IRetryService
    {
        Task Wait();

        void Reset();

        bool CanRetryFurther();
    }

    /// <summary>
    /// Service used as a strategy for waiting between failed requests
    /// Implements an exponential backoff; this works well when there's an outage, might not be the best for when throttled
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

        public async Task Wait()
        {
            if (_retryCount > MaxRetryCount)
            {
                throw new TimeoutException("Too many retries, giving up. Better luck next time.");
            }

            _retryCount++;

            var waitTime = _millisecondsToWaitNext + JitterMilliseconds;
            _logger.LogInformation($"Waiting for {waitTime} milliseconds");

            await Task.Delay(waitTime);

            _millisecondsToWaitNext *= BackoffFactor;
        }
    }
}
