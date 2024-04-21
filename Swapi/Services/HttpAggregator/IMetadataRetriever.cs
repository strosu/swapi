namespace Swapi.Services.HttpAggregator
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
        /// Retrieves multiple pages of results and aggregates the results; The retrieval in done sequentially
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> SequentiallyRetrieveObjectPagesAsync<T>(IEnumerable<string> pageUrls);
    }
}
