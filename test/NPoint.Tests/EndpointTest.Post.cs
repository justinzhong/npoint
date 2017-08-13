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
        public class Post
        {
            [Theory]
            [InlineData("http://example.com", "{'orderId':'3'}", null, "contentType")]
            [InlineData("http://example.com", null, "application/json", "body")]
            [InlineData(null, "{'orderId':'3'}", "application/json", "url")]
            public void ShouldNotAcceptNullArgs(string urlString, string body, string contentType, string paramName)
            {
                // Arrange
                var url = string.IsNullOrEmpty(urlString) ? default(Uri) : new Uri(urlString);

                // Act
                var sut = new Endpoint();
                Action activity = () => sut.Post(url, body, contentType);

                // Assert
                var assertion = activity.ShouldThrow<ArgumentException>();
                assertion.And.ParamName.ShouldBeEquivalentTo(paramName);
            }

            [Theory, NPointData(true)]
            public async Task ShouldSetEndpointHttpMethodAndContent(Uri url,
                IHttpRequestBuilder requestBuilder,
                IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestDispatcher requestDispatcher,
                string body,
                EndpointParameter parameter,
                HttpRequestMessage request,
                HttpResponseMessage response)
            {
                // Arrange
                string contentType = "application/json";
                var expectedHttpMethod = HttpMethod.Post;
                requestBuilder.Build().Returns(request);
                requestBuilder.SetUrl(url).Returns(requestBuilder);
                requestBuilder.SetHttpMethod(expectedHttpMethod).Returns(requestBuilder);
                requestBuilder.SetBody(body, contentType).Returns(requestBuilder);
                requestBuilderFactory.Create().Returns(requestBuilder);
                requestDispatcher.Dispatch(request, parameter.Timeout).Returns(Task.FromResult(response));

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter)
                    .Post(url, body, contentType);
                var actualResponse = await sut.CallThrough();

                // Assert
                requestBuilder.Received(1).SetUrl(Arg.Is(url));
                requestBuilder.Received(1).SetHttpMethod(Arg.Is(expectedHttpMethod));
                requestBuilder.Received(1).SetBody(Arg.Is(body), Arg.Is(contentType));
            }
        }
    }
}
