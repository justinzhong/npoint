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
        public class CallOfString
        {
            [Theory, NPointData(true)]
            public async Task ShouldPerformRequestSpecs(IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestDispatcher requestDispatcher,
                IHttpRequestBuilder requestBuilder,
                EndpointParameter parameter,
                Uri expectedUri,
                HttpContent expectedContent,
                HttpRequestMessage expectedRequest,
                HttpResponseMessage expectedResponse)
            {
                // Arrange
                var expectedBody = expectedContent.ReadAsStringAsync().Result;
                var expectedContentType = expectedContent.Headers.ContentType.MediaType;

                parameter.RequestSpecs = new List<Action<IHttpRequestBuilder>>
                {
                    { builder => builder.SetEndpoint(expectedUri) },
                    { builder => builder.SetBody(expectedBody, expectedContentType) }
                };
                requestBuilder.Build().Returns(expectedRequest);
                requestBuilderFactory.Create().Returns(requestBuilder);
                requestDispatcher.Dispatch(expectedRequest, parameter.Timeout).Returns(Task.FromResult(expectedResponse));

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter);
                await sut.Call();

                // Assert
                requestBuilder.Received(1).SetEndpoint(Arg.Is(expectedUri));
                requestBuilder.Received(1).SetBody(Arg.Is(expectedBody), Arg.Is(expectedContentType));
            }

            [Theory, NPointData(true)]
            public async Task ShouldInvokeCallback(IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestDispatcher requestDispatcher,
                IHttpRequestBuilder requestBuilder,
                EndpointParameter parameter,
                Uri expectedUri,
                HttpContent expectedContent,
                HttpRequestMessage expectedRequest,
                HttpResponseMessage expectedResponse)
            {
                // Arrange
                var callbackArg = default(HttpResponseMessage);
                Action<HttpResponseMessage> expectedCallback = response => callbackArg = response;

                parameter.OnResponseReceived = expectedCallback;
                parameter.RequestSpecs = new List<Action<IHttpRequestBuilder>>();
                requestBuilder.Build().Returns(expectedRequest);
                requestBuilderFactory.Create().Returns(requestBuilder);
                requestDispatcher.Dispatch(expectedRequest, parameter.Timeout).Returns(Task.FromResult(expectedResponse));

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter);
                await sut.Call();

                // Assert
                callbackArg.ShouldBeEquivalentTo(expectedResponse);
            }

            [Theory, NPointData(true)]
            public async Task ShouldReturnResponseBody(IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestDispatcher requestDispatcher,
                IHttpRequestBuilder requestBuilder,
                EndpointParameter parameter,
                Uri expectedUri,
                HttpContent expectedContent,
                HttpRequestMessage expectedRequest,
                HttpResponseMessage expectedResponse)
            {
                // Arrange
                parameter.RequestSpecs = new List<Action<IHttpRequestBuilder>>();
                requestBuilder.Build().Returns(expectedRequest);
                requestBuilderFactory.Create().Returns(requestBuilder);
                requestDispatcher.Dispatch(expectedRequest, parameter.Timeout).Returns(Task.FromResult(expectedResponse));

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter);
                var actualResponseBody = await sut.Call();

                // Assert
                var expectedResponseBody = expectedResponse.Content.ReadAsStringAsync().Result;
                actualResponseBody.ShouldBeEquivalentTo(expectedResponseBody);
            }
        }
    }
}
