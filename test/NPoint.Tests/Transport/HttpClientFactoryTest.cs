using FluentAssertions;
using System.Net.Http;
using Xunit;

namespace NPoint.Transport
{
    public class HttpClientFactoryTest
    {
        [Fact]
        public void ShouldCreateHttpClient()
        {
            // Arrange
            // Done

            // Act
            var sut = new HttpClientFactory();
            var actualHttpClient = sut.Create();

            // Assert
            actualHttpClient.Should().NotBeNull();
            actualHttpClient.Should().BeOfType<HttpClient>();
        }
    }
}
