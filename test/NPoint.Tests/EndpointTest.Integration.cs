using FluentAssertions;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace NPoint.Tests
{
    public partial class EndpointTest
    {
        public class Integration
        {
            [Fact]
            public async Task ShouldRetrieveWebPageViaGet()
            {
                // Arrange
                var googleNewsUrl = new Uri("https://news.google.com/news/headlines?ned=us&hl=en");

                // Act
                var result = await new Endpoint()
                    .Get(googleNewsUrl)
                    .CallThrough();

                // Assert
                result.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);
                result.Content.Should().NotBeNull();

                var body = await result.Content.ReadAsStringAsync();
                body.Should().NotBeNullOrWhiteSpace();
            }
        }
    }
}
