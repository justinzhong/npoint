using FluentAssertions;
using NPoint.Transport;
using NSubstitute;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NPoint.Tests
{
    public partial class EndpointTest
    {
        public class Delete
        {
            private readonly ITestOutputHelper output;

            public Delete(ITestOutputHelper output)
            {
                this.output = output;
            }

            [Fact]
            public void ShouldThrowNullArgumentException()
            {
                // Arrange
                var url = default(Uri);

                // Act
                var endpoint = new Endpoint();
                Action activity = () => endpoint.Delete(url);

                // Assert
                activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(nameof(url));
            }

            [Theory, NPointData(true)]
            public async Task ShouldApplyRequestSpecification(
                IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestBuilder requestBuilder,
                IHttpRequestDispatcher requestDispatcher,
                Uri expectedUrl,
                HttpResponseMessage expectedResponse)
            {
                // Arrange
                var expectedRequest = new HttpRequestMessage(HttpMethod.Delete, expectedUrl);
                var actualRequest = default(HttpRequestMessage);
                var actualTimeout = default(int);

                requestBuilder.Build().Returns(expectedRequest);
                requestBuilder.SetEndpoint(expectedUrl).Returns(requestBuilder);
                requestBuilderFactory.Create().Returns(requestBuilder);
                requestDispatcher.Dispatch(Arg.Do<HttpRequestMessage>(request => actualRequest = request), Arg.Do<int>(timeout => actualTimeout = timeout)).Returns(Task.FromResult(expectedResponse));

                // Act
                var actualResponse = await new Endpoint(requestBuilderFactory, requestDispatcher)
                    .Delete(expectedUrl)
                    .CallThrough();

                // Assert
                requestBuilder.Received(1).SetEndpoint(Arg.Is(expectedUrl));
                requestBuilder.Received(1).SetHttpMethod(Arg.Is(HttpMethod.Delete));
                actualTimeout.ShouldBeEquivalentTo(EndpointParameter.DefaultTimeout);
                actualRequest.RequestUri.ShouldBeEquivalentTo(expectedUrl);
            }
        }
    }
}
