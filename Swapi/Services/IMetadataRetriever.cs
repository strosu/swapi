using Swapi.Models;
using Swapi.Services.Http;

namespace Swapi.Services
{
    public interface IMetadataRetriever
    {
        /// <summary>
        /// Retrieves a single object, based on its url
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public Task<T> RetrieveObjectAsync<T>(string resource);

        /// <summary>
        /// Retrieves multiple pages of results and aggregates the results
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> RetrieveObjectPagesAsync<T>(IEnumerable<string> pageUrls);
    }

    public class MetadataRetriever(IRequestService requestService) : IMetadataRetriever
    {
        private readonly IRequestService _requestService = requestService;

        public async Task<T> RetrieveObjectAsync<T>(string resource)
        {
            return await _requestService.GetAsync<T>(resource);
        }

        public async Task<IEnumerable<T>> RetrieveObjectPagesAsync<T>(IEnumerable<string> pageUrls)
        {
            var results = new HashSet<T>();

            foreach (var url in pageUrls)
            {
                var pageResults = await _requestService.GetAsync<ResourceList<T>>(url);
                foreach (var listing in pageResults.Results)
                {
                    results.Add(listing);
                }
            }

            return results;
        }
    }
}
