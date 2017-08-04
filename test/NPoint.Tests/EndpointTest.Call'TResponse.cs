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
        public class CallOfTResponse
        {
            //[Fact]
            //public void ShouldThrowWhenArgumentIsNull()
            //{
            //    // Arrange
            //    var converter = default(Func<string, ResponseDto>);

            //    // Act
            //    var sut = new Endpoint();
            //    Func<Task<ResponseDto>> activity = async () => await sut.Call(converter);

            //    // Assert
            //    activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(nameof(converter));
            //}

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

            public class ResponseDto
            {
                public string Body { get; set; }
            }
        }
    }
}
