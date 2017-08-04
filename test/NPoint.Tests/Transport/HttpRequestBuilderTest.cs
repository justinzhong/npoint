using FluentAssertions;
using NPoint.Serialization;
using NPoint.Tests.Data;
using NPoint.Transport;
using NSubstitute;
using System;
using System.Collections.Specialized;
using System.Linq;
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

        [Theory, NPointData(true)]
        public void ShouldBuildRequestWithCustomHeaders(
            IUriQueryAppender queryAppender, 
            IJsonSerializer serializer)
        {
            // Arrange
            var customHeaders = new NameValueCollection
            {
                { "X-Param1", "Value1" },
                { "X-Param2", "Value2" },
                { "X-Param3", "Value3" },
                { "X-Param4", "Value4" },
                { "X-Param5", "Value5" },
            };

            // Act
            var sut = new HttpRequestBuilder(queryAppender, serializer)
                .SetHeaders(header =>
                {
                    foreach (var key in customHeaders.AllKeys) header.Add(key, customHeaders[key]);
                });
            var actual = sut.Build();

            // Assert
            foreach (var kv in actual.Headers)
            {
                customHeaders.AllKeys.Should().Contain(kv.Key);
                customHeaders[kv.Key].ShouldBeEquivalentTo(kv.Value.First());
            }
        }

        [Theory, NPointData(true)]
        public void ShouldBuildRequestMessageWithAllData(
            IUriQueryAppender queryAppender, 
            IJsonSerializer serializer,
            HttpRequestMessage request,
            Uri seedRequestUri,
            string expectedContentBody,
            string queryName,
            string queryValue)
        {
            // Arrange
            var expectedAccept = "application/octet-stream";
            var expectedContentType = "application/json";
            var expectedHttpMethod = HttpMethod.Options;
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
            queryAppender.AppendQuery(seedRequestUri, Arg.Any<NameValueCollection>()).Returns(firstPassUri);
            queryAppender.AppendQuery(firstPassUri, queryParams).Returns(expectedRequestUri.Uri);

            // Act
            var sut = new HttpRequestBuilder(queryAppender, serializer)
                .SetEndpoint(seedRequestUri)
                .SetAccept(expectedAccept)
                .SetBody(expectedContentBody, expectedContentType)
                .SetHttpMethod(expectedHttpMethod)
                .AddQuery(queryName, queryValue)
                .AddQuery(queryParams);
            var actual = sut.Build();

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
            var sut = new HttpRequestBuilder(queryAppender, serializer)
                .SetJson(payloadObject);
            var actual = sut.Build();

            // Assert
            actual.Content.ReadAsStringAsync().Result.Should().Be(expectedContentBody);
            actual.Content.Headers.ContentType.MediaType.Should().Be(expectedContentType);
            actual.Method.ShouldBeEquivalentTo(expectedHttpMethod);
        }
    }
}
