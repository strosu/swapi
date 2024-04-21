using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Swapi.Services.Http;

namespace Swapi.Tests
{
    public class RequestServiceTests
    {
        private RequestService requestService;
        private Mock<IHttpClientFactory> httpClientFactory = new Mock<IHttpClientFactory>();
        private Mock<IRetryService> retryService = new Mock<IRetryService>();
        private Mock<ILogger<RequestService>> logger = new Mock<ILogger<RequestService>>();
        private Mock<HttpClient> httpClient = new Mock<HttpClient>();

        public RequestServiceTests()
        {
            httpClientFactory.Setup(x => x.CreateClient(Options.DefaultName)).Returns(httpClient.Object);

            requestService = new RequestService(httpClientFactory.Object, retryService.Object, logger.Object);
        }

        [Fact]
        public async Task SendsRequestToUrl()
        {
            retryService.Setup(x => x.CanRetryFurther()).Returns(true);
            httpClient.Setup(x => x.GetAsync(It.IsAny<string>())).Returns(Task.FromResult(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
            }));

            var targetUrl = "someUrl";
            var result = await requestService.GetAsync<string>(targetUrl);
        }

    }
}
