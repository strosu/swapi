using Microsoft.AspNetCore.Http;
using Moq;
using StackExchange.Redis;
using Swapi.Middleware.RateLimiter;

namespace Swapi.Tests.Middleware
{
    public class RateLimiterPolicyTests
    {
        private Mock<IConnectionMultiplexer> _multiplexerMock = new Mock<IConnectionMultiplexer>();
        private Mock<IPartitionStrategy> _partitionStrategy = new Mock<IPartitionStrategy>();
        private PointQueryRateLimiterPolicy _policy;

        public RateLimiterPolicyTests()
        {
            _policy = new PointQueryRateLimiterPolicy(_multiplexerMock.Object, _partitionStrategy.Object);
        }

        [Fact] 
        public void GetPartition_Returns_SlidingWindowLimiter()
        {
            var contextMock = new Mock<HttpContext>();

            _partitionStrategy.Setup(x => x.GetPartition()).Returns("somePartition");

            var result = _policy.GetPartition(contextMock.Object);
            Assert.Equal("somePartition", result.PartitionKey);
        }
    }
}
