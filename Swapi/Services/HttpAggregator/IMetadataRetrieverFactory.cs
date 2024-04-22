namespace Swapi.Services.HttpAggregator
{
    public interface IMetadataRetrieverFactory
    {
        /// <summary>
        /// Enables getting multiple IMetadataRetriever instances
        /// </summary>
        /// <returns></returns>
        IMetadataRetriever CreateMetadataRetriever();
    }

    public class MetadataRetrieverFactory(
        IServiceProvider serviceProvider) : IMetadataRetrieverFactory
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public IMetadataRetriever CreateMetadataRetriever() => _serviceProvider.GetService<IMetadataRetriever>();
    }
}
