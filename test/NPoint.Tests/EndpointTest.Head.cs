﻿using FluentAssertions;
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
        public class Head
        {
            [Fact]
            public void ShouldNotAcceptNullUrl()
            {
                // Arrange
                var url = default(Uri);

                // Act
                var sut = new Endpoint();
                Action activity = () => sut.Head(url);

                // Assert
                activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(nameof(url));
            }

            [Theory, NPointData(true)]
            public async Task ShouldSetEndpointAndHttpHeadMethod(Uri url,
                IHttpRequestBuilder requestBuilder,
                IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestDispatcher requestDispatcher,
                EndpointParameter parameter,
                HttpRequestMessage request,
                HttpResponseMessage response)
            {
                // Arrange
                var expected = HttpMethod.Head;
                requestBuilder.Build().Returns(request);
                requestBuilder.SetUrl(url).Returns(requestBuilder);
                requestBuilder.SetHttpMethod(expected).Returns(requestBuilder);
                requestBuilderFactory.Create().Returns(requestBuilder);
                requestDispatcher.Dispatch(request, parameter.Timeout).Returns(Task.FromResult(response));

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter)
                    .Head(url);
                var actualResponse = await sut.CallThrough();

                // Assert
                requestBuilder.Received(1).SetUrl(Arg.Is(url));
                requestBuilder.Received(1).SetHttpMethod(Arg.Is(expected));
            }
        }
    }
}
