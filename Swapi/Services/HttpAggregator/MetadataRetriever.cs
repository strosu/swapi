using Swapi.Models.Repository;
using Swapi.Services.Http;

namespace Swapi.Services.HttpAggregator
{
    public class MetadataRetriever(IRequestService requestService) : IMetadataRetriever
    {
        private readonly IRequestService _requestService = requestService;

        public async Task<T> RetrieveObjectAsync<T>(string resource)
        {
            return await _requestService.GetAsync<T>(resource);
        }

        public async Task<IEnumerable<T>> SequentiallyRetrieveObjectPagesAsync<T>(IEnumerable<string> pageUrls)
        {
            var results = new HashSet<T>();

            foreach (var url in pageUrls)
            {
                var pageResults = await _requestService.GetAsync<ResourcePage<T>>(url);
                foreach (var listing in pageResults.Results)
                {
                    results.Add(listing);
                }
            }

            return results;
        }
    }
}
