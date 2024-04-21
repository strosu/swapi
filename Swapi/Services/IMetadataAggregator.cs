namespace Swapi.Services
{
    public interface IMetadataAggregator
    {
        public Task<T> GetSingleMetadataAsync<T>(int objectId);

        public Task<IEnumerable<T>> GetMetadataSetAsync<T>();
    }
}
