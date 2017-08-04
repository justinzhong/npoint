using FluentAssertions;
using NPoint.Serialization;
using NPoint.Tests.Data;
using NSubstitute;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace NPoint.Tests.Serialization
{
    public class JsonResponseConverterTest
    {
        [Fact]
        public void ShouldThrowArgumentNullExceptionFromCtor()
        {
            // Arrange
            var serializer = default(IJsonSerializer);

            // Act
            Action activity = () => new JsonResponseConverter(serializer);

            // Assert
            activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(nameof(serializer));
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionFromConvert()
        {
            // Arrange
            var response = default(HttpResponseMessage);

            // Act
            var sut = new JsonResponseConverter();
            Func<Task<CustomResponseModel>> activity = async () => await sut.Convert<CustomResponseModel>(response);

            // Assert
            activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(nameof(response));
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionFromValidateContent()
        {
            // Arrange
            var response = new HttpResponseMessage();

            //Act
            var sut = new JsonResponseConverter();
            Func<Task<CustomResponseModel>> activity = async () => await sut.Convert<CustomResponseModel>(response);

            // Assert
            var assertion = activity.ShouldThrowExactly<ArgumentException>();
            assertion.And.Message.Should().StartWith("Response has no content body");
            assertion.And.ParamName.ShouldBeEquivalentTo(nameof(response));
        }

        [Fact]
        public void ShouldThrowNotSupportedExceptionWhenContentTypeIsXml()
        {
            // Arrange
            var response = new HttpResponseMessage();
            var xmlContentType = "application/xml";
            response.Content = new StringContent(string.Empty);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(xmlContentType);

            //Act
            var sut = new JsonResponseConverter();
            Func<Task<CustomResponseModel>> activity = async () => await sut.Convert<CustomResponseModel>(response);

            // Assert
            var assertion = activity.ShouldThrowExactly<NotSupportedException>()
                .WithMessage($"The supported content type is application/json but the response content type is {xmlContentType}");
        }

        [Theory, NPointData(true)]
        public async Task ShouldConvertFromHttpResponseToResponseModel(
            HttpResponseMessage response, 
            IJsonSerializer serializer,
            CustomResponseModel expected)
        {
            // Arrange
            var body = response.Content.ReadAsStringAsync().Result;
            serializer.Deserialize<CustomResponseModel>(body).Returns(expected);

            //Act
            var sut = new JsonResponseConverter(serializer);
            var actual = await sut.Convert<CustomResponseModel>(response);

            // Assert
            actual.ShouldBeEquivalentTo(expected);
            serializer.Received(1).Deserialize<CustomResponseModel>(Arg.Is(body));
        }
    }
}
