using FluentAssertions;
using NPoint.Serialization;
using NPoint.Tests.Data;
using NPoint.Transport;
using NSubstitute;
using Ploeh.AutoFixture;
using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using Xunit;

namespace NPoint.Tests.Transport
{
    public class HttpRequestBuilderTest
    {
        [Theory]
        [InlineData(true, false, "serializer")]
        [InlineData(false, true, "queryAppender")]
        public void ShouldThrowExceptionWhenPassedNullArgToConstructor(bool queryAppenderSpecified, bool serializerSpecified, string argName)
        {
            // Arrange
            var queryAppender = queryAppenderSpecified ? Substitute.For<IUriQueryAppender>() : default(IUriQueryAppender);
            var serializer = serializerSpecified ? Substitute.For<IJsonSerializer>() : default(IJsonSerializer);

            // Act
            Action activity = () => new HttpRequestBuilder(queryAppender, serializer);

            // Assert
            activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.Should().Be(argName);
        }

        [Theory]
        [AutoNSubstituteData]
        public void ShouldThrowInvalidOperationWhenAddingQueryWithoutSettingEndpointFirst(IUriQueryAppender queryAppender, IJsonSerializer serializer)
        {
            // Arrange
            var fixture = new Fixture();
            var name = fixture.Create<string>();
            var value = fixture.Create<string>();

            // Act
            var sut = new HttpRequestBuilder(queryAppender, serializer);
            Action activity = () => sut.AddQuery(name, value);

            // Assert
            activity.ShouldThrowExactly<InvalidOperationException>().WithMessage("Request URI is null, call SetEndpoint first");
        }

        [Theory]
        [AutoNSubstituteData]
        public void ShouldThrowInvalidOperationWhenAddingBulkQueryWithoutSettingEndpointFirst(IUriQueryAppender queryAppender, IJsonSerializer serializer)
        {
            // Arrange
            var fixture = new Fixture();
            var nameValues = fixture.Create<NameValueCollection>();

            // Act
            var sut = new HttpRequestBuilder(queryAppender, serializer);
            Action activity = () => sut.AddQuery(nameValues);

            // Assert
            activity.ShouldThrowExactly<InvalidOperationException>().WithMessage("Request URI is null, call SetEndpoint first");
        }

        [Theory]
        [AutoNSubstituteData]
        public void ShouldBuildRequestMessageWithAllData(IUriQueryAppender queryAppender, IJsonSerializer serializer)
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new NPointCustomizations());
            var request = fixture.Create<HttpRequestMessage>();
            var seedRequestUri = fixture.Create<Uri>();
            var expectedAccept = "application/octet-stream";
            var expectedContentBody = fixture.Create<string>();
            var expectedContentType = "application/json";
            var expectedHttpMethod = HttpMethod.Options;
            var queryName = fixture.Create<string>();
            var queryValue = fixture.Create<string>();
            var queryParams = new NameValueCollection
            {
                { "Param1", "Value1" },
                { "Param2", "Value2" },
                { "Param3", "Value3" },
                { "Param4", "Value4" },
                { "Param5", "Value5" }
            };

            var expectedQueryBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(seedRequestUri.Query)) expectedQueryBuilder.Append(seedRequestUri + "&");

            expectedQueryBuilder.Append($"{Uri.EscapeDataString(queryName)}={Uri.EscapeDataString(queryValue)}&");

            foreach (var param in queryParams.AllKeys) expectedQueryBuilder.Append($"{Uri.EscapeDataString(param)}={Uri.EscapeDataString(queryParams[param])}&");

            expectedQueryBuilder.Length--;
            var expectedQuery = expectedQueryBuilder.ToString();
            var expectedRequestUri = new UriBuilder(seedRequestUri);
            expectedRequestUri.Query = expectedQuery;

            if (expectedRequestUri.Uri.IsDefaultPort) expectedRequestUri.Port = -1;

            var firstPassUri = new Uri($"{seedRequestUri.ToString()}&{queryName}={queryValue}");
            queryAppender.AppendQuery(seedRequestUri, queryName, queryValue).Returns(firstPassUri);
            queryAppender.AppendQuery(firstPassUri, queryParams).Returns(expectedRequestUri.Uri);

            // Act
            var sut = new HttpRequestBuilder(queryAppender, serializer);
            sut.SetEndpoint(seedRequestUri);
            sut.SetAccept(expectedAccept);
            sut.SetBody(expectedContentBody, expectedContentType);
            sut.SetHttpMethod(expectedHttpMethod);
            sut.AddQuery(queryName, queryValue);
            sut.AddQuery(queryParams);
            var actual = sut.Request;

            // Assert
            actual.RequestUri.Should().Be(expectedRequestUri.Uri);
            actual.Headers.Accept.Should().ContainSingle(acceptHeader => string.Equals(acceptHeader.MediaType, expectedAccept));
            actual.Content.ReadAsStringAsync().Result.Should().Be(expectedContentBody);
            actual.Content.Headers.ContentType.MediaType.Should().Be(expectedContentType);
            actual.Method.ShouldBeEquivalentTo(expectedHttpMethod);
        }

        [Theory, AutoNSubstituteData]
        public void ShouldSetJsonWithDefaultContentMimeType(IUriQueryAppender queryAppender, IJsonSerializer serializer)
        {
            // Arrange
            var expectedContentBody = "{ }";
            var payloadObject = new CustomResponseModel();
            serializer.Serialize(payloadObject).Returns(expectedContentBody);
            var expectedContentType = "application/json";
            var expectedHttpMethod = HttpMethod.Get;

            // Act
            var sut = new HttpRequestBuilder(queryAppender, serializer);
            sut.SetJson(payloadObject);
            var actual = sut.Request;

            // Assert
            actual.Content.ReadAsStringAsync().Result.Should().Be(expectedContentBody);
            actual.Content.Headers.ContentType.MediaType.Should().Be(expectedContentType);
            actual.Method.ShouldBeEquivalentTo(expectedHttpMethod);
        }

        [Theory, AutoNSubstituteData]
        public void ShouldSetJsonWithJsonpContentMimeType(IUriQueryAppender queryAppender, IJsonSerializer serializer)
        {
            // Arrange
            var expectedContentBody = "{ }";
            var payloadObject = new CustomResponseModel();
            serializer.Serialize(payloadObject).Returns(expectedContentBody);
            var expectedContentType = "application/jsonp";
            var expectedHttpMethod = HttpMethod.Get;

            // Act
            var sut = new HttpRequestBuilder(queryAppender, serializer);
            sut.SetJson(payloadObject, expectedContentType);
            var actual = sut.Request;

            // Assert
            actual.Content.ReadAsStringAsync().Result.Should().Be(expectedContentBody);
            actual.Content.Headers.ContentType.MediaType.Should().Be(expectedContentType);
            actual.Method.ShouldBeEquivalentTo(expectedHttpMethod);
        }
    }
}
