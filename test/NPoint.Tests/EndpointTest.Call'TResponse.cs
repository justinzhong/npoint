using FluentAssertions;
using NPoint.Tests.Data;
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
        public class CallOfTResponse
        {
            [Theory, NPointData(true)]
            public void ShouldThrowArgumentNullException(IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestDispatcher requestDispatcher,
                IHttpRequestBuilder requestBuilder,
                EndpointParameter parameter)
            {
                // Arrange
                var converter = default(Func<HttpResponseMessage, Task<CustomResponseModel>>);

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter);
                Func<Task<CustomResponseModel>> activity = async () => await sut.Call(converter);

                // Assert
                activity.ShouldThrowExactly<ArgumentNullException>()
                    .And.ParamName.ShouldBeEquivalentTo(nameof(converter));
            }

            [Theory, NPointData(true)]
            public async Task ShouldApplyOnResponseReceivedCallback(IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestDispatcher requestDispatcher,
                IHttpRequestBuilder requestBuilder,
                EndpointParameter parameter,
                HttpResponseMessage expectedHttpResponse,
                CustomResponseModel expectedConvertedResponse)
            {
                // Arrange
                Func<HttpResponseMessage, Task<CustomResponseModel>> converter = response => Task.FromResult(expectedConvertedResponse);
                var capturedResponse = default(HttpResponseMessage);
                parameter.OnResponseReceived = response => capturedResponse = response;
                requestDispatcher.Dispatch(Arg.Any<HttpRequestMessage>(), parameter.Timeout).Returns(Task.FromResult(expectedHttpResponse));

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter);
                var actual = await sut.Call(converter);

                // Assert
                actual.ShouldBeEquivalentTo(expectedConvertedResponse);
                capturedResponse.ShouldBeEquivalentTo(expectedHttpResponse);
            }

            [Theory, NPointData(true)]
            public async Task ShouldNotApplyOnResponseReceivedCallback(IHttpRequestBuilderFactory requestBuilderFactory,
                IHttpRequestDispatcher requestDispatcher,
                IHttpRequestBuilder requestBuilder,
                EndpointParameter parameter,
                HttpResponseMessage expectedHttpResponse,
                CustomResponseModel expectedConvertedResponse)
            {
                // Arrange
                Func<HttpResponseMessage, Task<CustomResponseModel>> converter = response => Task.FromResult(expectedConvertedResponse);
                parameter.OnResponseReceived = null;
                requestDispatcher.Dispatch(Arg.Any<HttpRequestMessage>(), parameter.Timeout).Returns(Task.FromResult(expectedHttpResponse));

                // Act
                var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter);
                var actual = await sut.Call(converter);

                // Assert
                actual.ShouldBeEquivalentTo(expectedConvertedResponse);
            }

            //[Theory, NPointData(true)]
            //public async Task ShouldApplyConverter(
            //    IHttpRequestBuilderFactory requestBuilderFactory,
            //    IHttpRequestDispatcher requestDispatcher,
            //    EndpointParameter parameter,
            //    IHttpRequestBuilder requestBuilder,
            //    HttpResponseMessage expectedResponse)
            //{
            //    // Arrange
            //    requestBuilderFactory.Create().Returns(requestBuilder);
            //    requestDispatcher.Dispatch(Arg.Any<HttpRequestMessage>(), Arg.Any<int>()).Returns(Task.FromResult(expectedResponse));
            //    Func<string, ResponseDto> converter = body => new ResponseDto { Body = body };

            //    // Act
            //    var sut = new Endpoint(requestBuilderFactory, requestDispatcher, parameter);
            //    var response = await sut.Call(converter);

            //    // Assert
            //    var expectedResponseBody = expectedResponse.Content.ReadAsStringAsync().Result;
            //    response.Body.ShouldBeEquivalentTo(expectedResponseBody);
            //}
        }
    }
}
