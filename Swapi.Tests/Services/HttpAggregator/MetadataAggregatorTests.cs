using Moq;
using Swapi.Models.Repository;
using Swapi.Services.HttpAggregator;

namespace Swapi.Tests.Services.HttpAggregator
{
    public class MetadataAggregatorTests
    {
        private Mock<IMetadataRetrieverFactory> _retrieverFactory = new Mock<IMetadataRetrieverFactory>();
        private Mock<IMetadataRetriever> _retriever = new Mock<IMetadataRetriever>();
        private MetadataAggregator _metadataAggregator;

        public MetadataAggregatorTests()
        {
            _retrieverFactory.Setup(x => x.CreateMetadataRetriever()).Returns(_retriever.Object);

            _metadataAggregator = new MetadataAggregator(_retrieverFactory.Object);
        }

        [Fact]
        public async Task GetSingleMetadataAsync_Delegates_To_Retriever()
        {
            var result = await _metadataAggregator.GetSingleMetadataAsync<SwapiPlanet>(1);
            _retriever.Verify(x => x.RetrieveObjectAsync<SwapiPlanet>("https://swapi.dev/api/planets/1"), Times.Once);
        }

        [Fact]
        public async Task GetMetadataSetAsync_Takes_All_Pages()
        {
            var firstPage = new ResourcePage<SwapiPlanet> {
                Count = 11,
                Results = new List<SwapiPlanet> {
                    new SwapiPlanet() {
                        IdentifyingUrl = "firstUrl"
                    }
                }
            };

            var secondPage = new ResourcePage<SwapiPlanet>
            {
                Results = new List<SwapiPlanet> {
                    new SwapiPlanet() {
                        IdentifyingUrl = "secondUrl"
                    }
                }
            };

            _retriever.Setup(x => x.RetrieveObjectAsync<ResourcePage<SwapiPlanet>>("https://swapi.dev/api/planets/?page=1"))
                .Returns(Task.FromResult(firstPage));

            _retriever.Setup(x => x.SequentiallyRetrieveObjectPagesAsync<SwapiPlanet>(
                new[] { "https://swapi.dev/api/planets/?page=1" }))
                .Returns(Task.FromResult(firstPage.Results.AsEnumerable()));

            _retriever.Setup(x => x.SequentiallyRetrieveObjectPagesAsync<SwapiPlanet>(
                new[] { "https://swapi.dev/api/planets/?page=2" }))
                .Returns(Task.FromResult(secondPage.Results.AsEnumerable()));

            var result = await _metadataAggregator.GetMetadataSetAsync<SwapiPlanet>();
            Assert.Equal(2, result.Count());
            Assert.Equal("firstUrl", result.ElementAt(0).IdentifyingUrl);
            Assert.Equal("secondUrl", result.ElementAt(1).IdentifyingUrl);
        }
    }
}
