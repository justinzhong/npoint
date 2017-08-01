using FluentAssertions;
using NPoint.Transport;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace NPoint.Tests
{
    public partial class EndpointTest
    {
        public class CallString
        {
            [Theory, NPointData(true)]
            public async Task ShouldPerformRequestSpecs(IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestDispatcher requestDispatcher,
                EndpointParameter parameter,
                Uri expectedUri,
                HttpContent expectedContent,
                HttpResponseMessage expectedResponse)
            {
                // Arrange
                var expectedBody = expectedContent.ReadAsStringAsync().Result;
                var expectedContentType = expectedContent.Headers.ContentType.MediaType;
                var actualRequest = default(HttpRequestMessage);

                parameter.RequestSpecs = new List<Action<IHttpRequestBuilder>>
                {
                    { builder => builder.SetEndpoint(expectedUri) },
                    { builder => builder.SetBody(expectedBody, expectedContentType) }
                };
                requestDispatcher.Dispatch(Arg.Do<HttpRequestMessage>(request => actualRequest = request), parameter.Timeout)
                    .Returns(Task.FromResult(expectedResponse));

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter);
                var response = await sut.Call();

                // Assert
                var actualBody = actualRequest.Content.ReadAsStringAsync().Result;
                var actualContentType = actualRequest.Content.Headers.ContentType.MediaType;

                actualRequest.RequestUri.ShouldBeEquivalentTo(expectedUri);
                actualBody.ShouldBeEquivalentTo(expectedBody);
                actualContentType.ShouldBeEquivalentTo(expectedContentType);
                response.ShouldBeEquivalentTo(expectedResponse);
            }
        }
    }
}
