using Swapi.Models;

namespace Swapi.Services
{
    public interface IMetadataFinder
    {
        public Task<T> GetSingleMetadata<T>(int objectId);

        public Task<T> GetMetadatList<T>();
    }

    public class MetadataFinder(MetadataRetriever metadataRetriever) : IMetadataFinder
    {
        private readonly MetadataRetriever _metadataRetriever = metadataRetriever;

        public Task<T> GetMetadatList<T>()
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetSingleMetadata<T>(int objectId)
        {
            var url = GetUrl<T>(objectId);

            return await _metadataRetriever.RetrieveObject<T>(url);
        }

        private string GetUrl<T>(int? id)
        {
            var currentType = typeof(T);

            if (!MetadataConfiguration.EntityMapping.ContainsKey(currentType))
            {
                throw new NotImplementedException($"No implementation provided for type {currentType}, " +
                    $"please configure it in {nameof(MetadataConfiguration.EntityMapping)}");
            }

            if (id == null)
            {
                return MetadataConfiguration.EntityMapping[currentType];
            }

            return $"{MetadataConfiguration.EntityMapping}/{id}";
        }
    }
}
