using FluentAssertions;
using NPoint.Transport;
using NSubstitute;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace NPoint.Tests
{
    public partial class EndpointTest
    {
        public class SetHeader
        {
            [Fact]
            public void ShouldNotAcceptNullArg()
            {
                // Arrange
                var headerSpec = default(Action<HttpRequestHeaders>);

                // Act
                var sut = new Endpoint();
                Action activity = () => sut.SetHeader(headerSpec);

                // Assert
                activity.ShouldThrowExactly<ArgumentNullException>()
                    .And.ParamName.ShouldBeEquivalentTo(nameof(headerSpec));
            }

            [Theory, NPointData(true)]
            public async Task ShouldSetHeaders(Uri url,
                IHttpRequestBuilder requestBuilder,
                IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestDispatcher requestDispatcher,
                string body,
                HttpRequestMessage request,
                HttpResponseMessage expectedResponse,
                EndpointParameter parameter,
                HttpRequestMessage expected)
            {
                // Arrange
                var customHeaders = new NameValueCollection
                {
                    { "X-Custom-Param1", "Value1" },
                    { "X-Custom-Param2", "Value2" },
                    { "X-Custom-Param3", "Value3" },
                };
                Action<HttpRequestHeaders> headerSpec = headers =>
                {
                    foreach (var param in customHeaders.AllKeys) headers.Add(param, customHeaders[param]);
                };
                requestBuilder.SetHeader(headerSpec).Returns(_ => {
                    headerSpec(request.Headers);

                    return requestBuilder;
                });
                requestBuilder.Build().Returns(request);
                requestBuilderFactory.Create().Returns(requestBuilder);
                requestDispatcher.Dispatch(request, parameter.Timeout).Returns(Task.FromResult(expectedResponse));

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter)
                    .SetHeader(headerSpec);
                var actualResponse = await sut.CallThrough();

                // Assert
                foreach (var param in customHeaders.AllKeys)
                {
                    request.Headers.Should().ContainSingle(kv => Equals(kv.Key, param) && Equals(kv.Value.First(), customHeaders[param]));
                }
            }
        }
    }
}
