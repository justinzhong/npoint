using FluentAssertions;
using NPoint.Transport;
using NSubstitute;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace NPoint.Tests
{
    public partial class EndpointTest
    {
        public class OnReceived
        {
            [Fact]
            public void ShouldNotAcceptNullArg()
            {
                // Arrange
                var callback = default(Action<HttpResponseMessage>);

                // Act
                var sut = new Endpoint();
                Action activity = () => sut.OnReceived(callback);

                // Assert
                activity.ShouldThrowExactly<ArgumentNullException>()
                    .And.ParamName.ShouldBeEquivalentTo(nameof(callback));
            }

            [Theory, NPointData(true)]
            public async Task ShouldSetOnReceivedCallback(Uri url,
                IHttpRequestBuilder requestBuilder,
                IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestDispatcher requestDispatcher,
                string body,
                EndpointParameter parameter,
                HttpRequestMessage request,
                HttpResponseMessage expectedResponse)
            {
                // Arrange
                var captured = default(HttpResponseMessage);
                Action<HttpResponseMessage> callback = message => captured = message;
                requestBuilder.Build().Returns(request);
                requestBuilderFactory.Create().Returns(requestBuilder);
                requestDispatcher.Dispatch(request, parameter.Timeout).Returns(Task.FromResult(expectedResponse));

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter)
                    .OnReceived(callback);
                var actual = await sut.CallThrough();

                // Assert
                actual.ShouldBeEquivalentTo(expectedResponse, "they are the same HTTP response message");
                captured.ShouldBeEquivalentTo(expectedResponse, "the callback should have been invoked and captured the same HTTP response message");
            }
        }
    }
}
