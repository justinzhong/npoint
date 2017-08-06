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
        public class TimeoutWhen
        {
            [Theory, NPointData]
            public void ShouldThrowExceptionWhenNegativeTimeoutIsSpecified(int timeout)
            {
                // Arrange
                var negativeTimeout = Math.Abs(timeout) * -1;

                // Act
                var sut = new Endpoint();
                Action activity = () => sut.TimeoutWhen(negativeTimeout);

                // Assert
                var assertion = activity.ShouldThrowExactly<ArgumentOutOfRangeException>();
                assertion.And.ParamName.ShouldBeEquivalentTo(nameof(timeout));
                assertion.And.Message.Should().StartWith("Timeout must be a non-negative value");
            }

            [Theory, NPointData(true)]
            public async Task ShouldSetTimeout(
                IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestBuilder requestBuilder,
                IHttpRequestDispatcher requestDispatcher,
                int timeout,
                HttpRequestMessage request,
                HttpResponseMessage response)
            {
                // Arrange
                requestBuilder.Build().Returns(request);
                requestBuilderFactory.Create().Returns(requestBuilder);
                requestDispatcher.Dispatch(request, timeout).Returns(Task.FromResult(response));

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher)
                    .TimeoutWhen(timeout);
                var actualResponse = await sut.CallThrough();

                // Assert
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                requestDispatcher.Received(1).Dispatch(Arg.Is(request), Arg.Is(timeout));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                actualResponse.ShouldBeEquivalentTo(response);
            }

            [Theory, NPointData(true)]
            public async Task ShouldAggregateSpecs(
                IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestBuilder requestBuilder,
                IHttpRequestDispatcher requestDispatcher,
                int timeout,
                HttpRequestMessage request,
                HttpResponseMessage response)
            {
                // Arrange
                request.Method = HttpMethod.Delete;
                requestBuilder.SetEndpoint(request.RequestUri).Returns(requestBuilder);
                requestBuilder.SetHttpMethod(HttpMethod.Delete).Returns(requestBuilder);
                requestBuilder.Build().Returns(request);
                requestBuilderFactory.Create().Returns(requestBuilder);
                requestDispatcher.Dispatch(request, timeout).Returns(Task.FromResult(response));

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher)
                    .Delete(request.RequestUri)
                    .TimeoutWhen(timeout);
                var actualResponse = await sut.CallThrough();

                // Assert
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                requestDispatcher.Received(1).Dispatch(Arg.Is(request), Arg.Is(timeout));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                actualResponse.ShouldBeEquivalentTo(response);
                requestBuilder.Received(1).SetEndpoint(Arg.Is(request.RequestUri));
                requestBuilder.Received(1).SetHttpMethod(Arg.Is(HttpMethod.Delete));
            }
        }
    }
}
