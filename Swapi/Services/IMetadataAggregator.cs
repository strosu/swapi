using Swapi.Models;
using Swapi.Models.Repository;

namespace Swapi.Services
{
    public interface IMetadataAggregator
    {
        public Task<T> GetSingleMetadataAsync<T>(int objectId);

        public Task<IEnumerable<T>> GetMetadataSetAsync<T>();
    }

    public class MetadataAggregator(IMetadataRetrieverFactory metadataRetrieverFactory) : IMetadataAggregator
    {
        private static readonly int MaxDegreeOfParallelism = 5;
        private static readonly int ResultsPerPage = 10;

        private readonly IMetadataRetrieverFactory _metadataRetrieverFactory = metadataRetrieverFactory;

        public async Task<IEnumerable<T>> GetMetadataSetAsync<T>()
        {
            var pageDistribution = await Paginate<T>();
            var taskList = new List<Task<IEnumerable<T>>>();

            foreach (var urlList in pageDistribution)
            {
                var retriever = _metadataRetrieverFactory.CreateMetadataRetriever();
                taskList.Add(retriever.SequentiallyRetrieveObjectPagesAsync<T>(urlList));
            }

            await Task.WhenAll(taskList);

            // Flatten the list of results
            return taskList.Select(x => x.Result).SelectMany(x => x);
        }

        public async Task<T> GetSingleMetadataAsync<T>(int objectId)
        {
            var url = MetadataConfiguration.GetEntityUrl<T>(objectId);

            return await _metadataRetrieverFactory.CreateMetadataRetriever()
                .RetrieveObjectAsync<T>(url);
        }

        /// <summary>
        /// Paginates a list of URLs into multiple buckets, as evenly split as possible
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private async Task<IEnumerable<IEnumerable<string>>> Paginate<T>()
        {
            var pageCount = await GetNumberOfPages<T>();
            var result = new List<List<string>>();

            // If there are less pages than the degree of parallelism, no need to spawn additional retrievers
            for (var i = 0; i < Math.Min(MaxDegreeOfParallelism, pageCount); i++)
            {
                result.Add([]);
            }

            for (var i = 1; i <= pageCount; i++)
            {
                var pageUrl = MetadataConfiguration.GetEntityPage<T>(i);
                var crawlerID = (i - 1) % result.Count;
                result[crawlerID].Add(pageUrl);
            }

            return result;
        }

        private async Task<int> GetNumberOfPages<T>()
        {
            var url = MetadataConfiguration.GetEntityPage<T>(1);
            var firstPage = await _metadataRetrieverFactory
                .CreateMetadataRetriever().RetrieveObjectAsync<ResourceList<T>>(url);
            var totalResourceCount = firstPage.Count;

            return Utils.RoundUpDivision(totalResourceCount, ResultsPerPage);
        }
    }
}
