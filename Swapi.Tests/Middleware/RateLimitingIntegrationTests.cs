using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Swapi.Tests
{
    public class RateLimitingIntegrationTests: IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        private readonly string SingleGetPath = "/planets/1";
        private readonly string AggregateGetPath = "/planets";

        public RateLimitingIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateClient(options: new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost:7003")
            });
        }

        [Fact]
        public async Task When_No_Header_All_Share_Same_Limit()
        {
            var requests = new List<Task<HttpStatusCode>>();
            // The limiter has an upper bound of 2 requests per 5s per partition
            // Add more to verify at one is rejected
            for (var i = 0; i < 3; i++)
            {
                requests.Add(GetResult(SingleGetPath));
            }

            await Task.WhenAll(requests);
            Assert.Equal(2, requests.Count(x => x.Result == HttpStatusCode.OK));
            Assert.Equal(1, requests.Count(x => x.Result == HttpStatusCode.TooManyRequests));
        }

        [Fact]
        public async Task When_Header_Set_Each_Has_Own_Limit()
        {
            var forwardedFor1 = "127.0.0.1";
            var forwardedFor2 = "127.0.0.2";
            var requests = new List<Task<HttpStatusCode>>
            {
                GetResult(SingleGetPath, forwardedFor1),
                GetResult(SingleGetPath, forwardedFor1),
                GetResult(SingleGetPath, forwardedFor2)
            };

            await Task.WhenAll(requests);

            Assert.Equal(3, requests.Count(x => x.Result == HttpStatusCode.OK));
            Assert.Equal(0, requests.Count(x => x.Result == HttpStatusCode.TooManyRequests));
        }

        private async Task<HttpStatusCode> GetResult(string path, string? header = null)
        {
            using var request = new HttpRequestMessage(new HttpMethod("GET"), path);
            if (header != null)
            {
                request.Headers.Add("X-Forwarded-For", header);
            }

            using var response = await _httpClient.SendAsync(request);

            return response.StatusCode;
        }
    }
}
