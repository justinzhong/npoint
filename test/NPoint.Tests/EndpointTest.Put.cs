using FluentAssertions;
using NPoint.Transport;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NPoint.Tests
{
    public partial class EndpointTest
    {
        public class Put
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
                Action activity = () => sut.Put(url, body, contentType);

                // Assert
                var assertion = activity.ShouldThrow<ArgumentException>();
                assertion.And.ParamName.ShouldBeEquivalentTo(paramName);
            }

            [Theory, NPointData(true)]
            public void ShouldSetEndpointHttpMethodAndContent(Uri url,
                IHttpRequestBuilder requestBuilder,
                IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestDispatcher requestDispatcher,
                string body,
                EndpointParameter parameter)
            {
                // Arrange
                string contentType = "application/json";
                var expectedHttpMethod = HttpMethod.Put;
                requestBuilder.SetEndpoint(url).Returns(requestBuilder);
                requestBuilder.SetHttpMethod(expectedHttpMethod).Returns(requestBuilder);
                requestBuilder.SetBody(body, contentType).Returns(requestBuilder);
                requestBuilderFactory.Create().Returns(requestBuilder);

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter)
                    .Put(url, body, contentType);
                parameter.RequestSpecs.ForEach(spec => spec(requestBuilder));

                // Assert
                parameter.RequestSpecs.Count.ShouldBeEquivalentTo(1);
                requestBuilder.Received(1).SetEndpoint(Arg.Is(url));
                requestBuilder.Received(1).SetHttpMethod(Arg.Is(expectedHttpMethod));
                requestBuilder.Received(1).SetBody(Arg.Is(body), Arg.Is(contentType));
            }
        }
    }
}
