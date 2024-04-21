namespace Swapi.Services.HttpAggregator
{
    public interface IMetadataRetrieverFactory
    {
        IMetadataRetriever CreateMetadataRetriever();
    }

    public class MetadataRetrieverFactory(
        IServiceProvider serviceProvider) : IMetadataRetrieverFactory
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public IMetadataRetriever CreateMetadataRetriever() => _serviceProvider.GetService<IMetadataRetriever>();
    }
}
