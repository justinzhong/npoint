using FluentAssertions;
using NPoint.Transport;
using NSubstitute;
using System;
using System.Net.Http;
using Xunit;

namespace NPoint.Tests
{
    public partial class EndpointTest
    {
        public class Get
        {
            [Fact]
            public void ShouldNotAcceptNullUrl()
            {
                // Arrange
                var url = default(Uri);

                // Act
                var sut = new Endpoint();
                Action activity = () => sut.Get(url);

                // Assert
                activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(nameof(url));
            }

            [Theory, NPointData(true)]
            public void ShouldSetEndpointAndHttpGetMethod(Uri url, 
                IHttpRequestBuilder requestBuilder, 
                IHttpRequestBuilderFactory requestBuilderFactory, 
                IHttpRequestDispatcher requestDispatcher,
                EndpointParameter parameter)
            {
                // Arrange
                var expected = HttpMethod.Get;
                requestBuilder.SetEndpoint(url).Returns(requestBuilder);
                requestBuilder.SetHttpMethod(HttpMethod.Get).Returns(requestBuilder);
                requestBuilderFactory.Create().Returns(requestBuilder);

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter)
                    .Get(url);
                parameter.RequestSpecs.ForEach(spec => spec(requestBuilder));

                // Assert
                parameter.RequestSpecs.Count.ShouldBeEquivalentTo(1);
                requestBuilder.Received(1).SetEndpoint(Arg.Is(url));
                requestBuilder.Received(1).SetHttpMethod(Arg.Is(HttpMethod.Get));
            }
        }
    }
}
