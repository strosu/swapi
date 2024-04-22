using System.Text.Json;

namespace Swapi.Services.Http
{
    public interface IRequestService
    {
        /// <summary>
        /// Performs an HTTP get to a URL and tries to cast it into the type asked for.
        /// Abstracts away retries, getting throttled etc.
        /// </summary>
        /// <typeparam name="T">The type of the object the response should be deserialized into</typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string url);
    }

    public class RequestService(
        IHttpClientFactory httpClientFactory, 
        IRetryService retryService, 
        ILogger<RequestService> logger) : IRequestService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IRetryService _retryService = retryService;
        private readonly ILogger<RequestService> _logger = logger;


        public async Task<T> GetAsync<T>(string url)
        {
            _logger.LogInformation($"Retrieving ${url}");
            _retryService.Reset();

            using var httpClient = _httpClientFactory.CreateClient();
            HttpResponseMessage? result = null;

            while (_retryService.CanRetryFurther())
            {
                try
                {
                    result = await httpClient.GetAsync(url);

                    if (result != null && result.IsSuccessStatusCode)
                    {
                        var json = await result.Content.ReadAsStringAsync();

                        _logger.LogInformation($"Finished getting page {url}");

                        return JsonSerializer.Deserialize<T>(json);
                    }
                }
                catch (Exception ex)
                {
                    // This is meant to catch unhandled exceptions, e.g. server throwing a 500 for whatever reason; In that case, don't just give up
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.InnerException?.Message);
                }

                await _retryService.Wait(result);
            }

            var errorMessage = "Could not get the request in time, giving up.";
            _logger.LogError(errorMessage);
            throw new TimeoutException(errorMessage);
        }
    }
}
