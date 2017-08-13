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
        public class RequestWith
        {
            public class RequestAsArg
            {
                [Fact]
                public void ShouldNotAcceptNullArg()
                {
                    // Arrange
                    var request = default(HttpRequestMessage);

                    // Act
                    var sut = new Endpoint();
                    Action activity = () => sut.RequestWith(request);

                    // Assert
                    activity.ShouldThrowExactly<ArgumentNullException>()
                        .And.ParamName.ShouldBeEquivalentTo(nameof(request));
                }

                [Theory, NPointData(true)]
                public async Task ShouldSetRequest(Uri url,
                    IHttpRequestBuilder requestBuilder,
                    IHttpRequestBuilderFactory requestBuilderFactory,
                    IHttpRequestDispatcher requestDispatcher,
                    string body,
                    EndpointParameter parameter,
                    HttpRequestMessage expected,
                    HttpResponseMessage response)
                {
                    // Arrange
                    requestBuilder.Build().Returns(expected);
                    requestBuilder.SetRequest(expected).Returns(requestBuilder);
                    requestBuilderFactory.Create().Returns(requestBuilder);
                    requestDispatcher.Dispatch(expected, parameter.Timeout).Returns(Task.FromResult(response));

                    // Act
                    var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter)
                        .RequestWith(expected);
                    var actualResponse = await sut.CallThrough();

                    // Assert
                    requestBuilder.Received(1).SetRequest(Arg.Is(expected));
                }
            }

            public class ActionAsArg
            {
                [Fact]
                public void ShouldNotAcceptNullArg()
                {
                    // Arrange
                    var requestSpec = default(Action<IHttpRequestBuilder>);

                    // Act
                    var sut = new Endpoint();
                    Action activity = () => sut.RequestWith(requestSpec);

                    // Assert
                    activity.ShouldThrowExactly<ArgumentNullException>()
                        .And.ParamName.ShouldBeEquivalentTo(nameof(requestSpec));
                }

                [Theory, NPointData(true)]
                public async Task ShouldSetRequest(Uri url,
                    IHttpRequestBuilder requestBuilder,
                    IHttpRequestBuilderFactory requestBuilderFactory,
                    IHttpRequestDispatcher requestDispatcher,
                    string body,
                    HttpRequestMessage request,
                    HttpResponseMessage response,
                    EndpointParameter parameter)
                {
                    // Arrange
                    var expected = HttpMethod.Trace;
                    requestBuilder.SetUrl(url).Returns(requestBuilder);
                    requestBuilder.SetHttpMethod(expected).Returns(requestBuilder);
                    requestBuilder.Build().Returns(request);
                    requestBuilderFactory.Create().Returns(requestBuilder);
                    requestDispatcher.Dispatch(request, parameter.Timeout).Returns(Task.FromResult(response));

                    // Act
                    var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter)
                        .RequestWith(builder => builder.SetUrl(url).SetHttpMethod(expected));
                    var actualResponse = await sut.CallThrough();

                    // Assert
                    requestBuilder.Received(1).SetUrl(Arg.Is(url));
                    requestBuilder.Received(1).SetHttpMethod(expected);
                }
            }
        }
    }
}
