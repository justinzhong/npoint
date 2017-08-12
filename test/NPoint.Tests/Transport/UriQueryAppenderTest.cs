using FluentAssertions;
using NPoint.Transport;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Xunit;

namespace NPoint.Tests.Transport
{
    public class UriQueryAppenderTest
    {
        [Theory]
        [InlineData("http://api-endpoint/", "acquired ticket", "$+@3*&^g-?a\\", "http://api-endpoint/?acquired%20ticket=%24%2B%403%2A%26%5Eg-%3Fa%5C")]
        [InlineData("https://api-endpoint/service?page=2", "acquired ticket", "$+@3*&^g-?a\\", "https://api-endpoint/service?page=2&acquired%20ticket=%24%2B%403%2A%26%5Eg-%3Fa%5C")]
        [InlineData("https://api-endpoint:8080/service/?page=2", "acquired ticket", "$+@3*&^g-?a\\", "https://api-endpoint:8080/service/?page=2&acquired%20ticket=%24%2B%403%2A%26%5Eg-%3Fa%5C")]
        [InlineData("https://api-endpoint/service?name=%3A%29&page=2", "acquired ticket", "$+@3*&^g-?a\\", "https://api-endpoint/service?name=%3A%29&page=2&acquired%20ticket=%24%2B%403%2A%26%5Eg-%3Fa%5C")]
        [InlineData("https://api-endpoint/service?name=%3A%29&page=2#top", "acquired ticket", "$+@3*&^g-?a\\", "https://api-endpoint/service?name=%3A%29&page=2&acquired%20ticket=%24%2B%403%2A%26%5Eg-%3Fa%5C#top")]
        [InlineData("service/resource/category?page=2", "acquired ticket", "$+@3*&^g-?a\\", "service/resource/category?page=2&acquired%20ticket=%24%2B%403%2A%26%5Eg-%3Fa%5C")]
        public void ShouldAppendNameAndValueToQuery(string seedUrlString, string paramName, string paramValue, string expectedUrlString)
        {
            ShouldAppendNameAndValuesToQueryCore(seedUrlString, new NameValueCollection { { paramName, paramValue } }, expectedUrlString);
        }

        [Fact]
        public void ShouldAppendBulkNameAndValuesToQuery()
        {
            // Arrange
            var seedUrlString = @"https://api-endpoint:2376";
            var nameValues = new NameValueCollection
            {
                { "user", "email@example.com" },
                { "pass", "$+@3*&^g-?a\\" },
                { "acorf-token", "YXBpLXNlcnZlcg==" }
            };
            var urlBuilder = new StringBuilder(seedUrlString);
            urlBuilder.Append("?");

            foreach (var name in nameValues.AllKeys)
            {
                urlBuilder.Append($"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(nameValues[name])}&");
            }

            urlBuilder.Length--;
            var expectedUrlString = urlBuilder.ToString();

            // Act & Assert
            ShouldAppendNameAndValuesToQueryCore(seedUrlString, nameValues, expectedUrlString);
        }

        [Theory, NPointData()]
        public void ShouldAppendNameAndValue(Uri seedUrl, string name, string value)
        {
            // Arrange
            var urlBuilder = new StringBuilder(seedUrl.ToString());
            urlBuilder.Append("?");
            urlBuilder.Append($"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(value)}&");
            urlBuilder.Length--;
            var expected = urlBuilder.ToString();

            // Act
            var sut = new UriQueryAppender();
            var actual = sut.AppendQuery(seedUrl, name, value);

            // Assert
            actual.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldAppendBulkNameAndValuesToQueryWithComplexUrl()
        {
            // Arrange
            var seedUrlString = @"https://api-endpoint:2376/service-a/category-123/?from=101&to=200&display%20name=%3A%29#%24filter";
            var nameValues = new NameValueCollection
            {
                { "user", "email@example.com" },
                { "pass", "$+@3*&^g-?a\\" },
                { "acorf-token", "YXBpLXNlcnZlcg==" }
            };
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("&");

            foreach (var name in nameValues.AllKeys)
            {
                queryBuilder.Append($"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(nameValues[name])}&");
            }

            queryBuilder.Length--;
            var expectedUrlString = $@"https://api-endpoint:2376/service-a/category-123/?from=101&to=200&display%20name=%3A%29{queryBuilder.ToString()}#%24filter";

            // Act & Assert
            ShouldAppendNameAndValuesToQueryCore(seedUrlString, nameValues, expectedUrlString);
        }

        [Theory, NPointData()]
        public void ShouldReturnOriginalUrlWithEmptyNameValues(Uri seedUrl)
        {
            // Arrange
            var nameValues = new NameValueCollection();

            // Act
            var sut = new UriQueryAppender();
            var actual = sut.AppendQuery(seedUrl, nameValues);

            // Assert
            actual.ShouldBeEquivalentTo(seedUrl);
        }

        private void ShouldAppendNameAndValuesToQueryCore(string seedUrlString, NameValueCollection nameValues, string expectedUrlString)
        {
            // Arrange
            var existingUrl = new Uri(seedUrlString, UriKind.RelativeOrAbsolute);
            var expectedUrl = new Uri(expectedUrlString, UriKind.RelativeOrAbsolute);
            var existingQuery = seedUrlString.Contains("?") ? seedUrlString.Substring(seedUrlString.IndexOf("?")) : string.Empty;
            var existingFragments = string.Empty;

            if (existingQuery.Contains("#"))
            {
                existingFragments = existingQuery.Substring(existingQuery.IndexOf("#"));
                existingQuery = existingQuery.Substring(0, existingQuery.IndexOf("#"));
            }

            // Act
            var sut = new UriQueryAppender();
            var actualUrl = sut.AppendQuery(existingUrl, nameValues);

            // Assert
            actualUrl.ShouldBeEquivalentTo(expectedUrl);
            actualUrl.OriginalString.ShouldBeEquivalentTo(expectedUrlString);
            var actualUrlString = actualUrl.OriginalString;
            var actualQuery = actualUrlString.Substring(actualUrlString.IndexOf("?"));
            var actualFragments = string.Empty;

            if (actualUrlString.Contains("#"))
            {
                actualFragments = actualQuery.Substring(actualQuery.IndexOf("#"));
                actualQuery = actualQuery.Substring(0, actualQuery.IndexOf("#"));
            }

            var parsedQuery = actualQuery.Substring(existingQuery.Length)
                .TrimStart('?', '&')
                .Split('&')
                .ToList();

            for (var i = 0; i < parsedQuery.Count; i++)
            {
                var queryPart = parsedQuery[i];
                var paramName = nameValues.Keys[i];
                var paramValue = nameValues[paramName];
                
                var unescapedQuery = queryPart.Split('=').ToList()
                .Select(str => Uri.UnescapeDataString(str))
                .ToArray();

                unescapedQuery[0].ShouldBeEquivalentTo(paramName);
                unescapedQuery[1].ShouldBeEquivalentTo(paramValue);
            }

            if (!string.IsNullOrEmpty(existingFragments)) actualFragments.ShouldBeEquivalentTo(existingFragments);
        }

        public class ShouldThrowArgumentException
        {
            public class AddQueryWithString
            {
                [Theory]
                [InlineData("http://example.com", "parameter", null, typeof(ArgumentException), "value")]
                [InlineData("http://example.com", null, "value", typeof(ArgumentException), "name")]
                [InlineData(null, "parameter", "value", typeof(ArgumentNullException), "url")]
                public void ShouldThrowArgumentException(string urlString, string parameter, string value, Type exceptionType, string paramName)
                {
                    // Arrange
                    var url = string.IsNullOrEmpty(urlString) ? default(Uri) : new Uri(urlString);

                    // Act
                    var sut = new UriQueryAppender();
                    Action activity = () => sut.AppendQuery(url, parameter, value);

                    // Assert
                    var assertion = activity.ShouldThrow<ArgumentException>();
                    assertion.And.Should().BeOfType(exceptionType);
                    assertion.And.ParamName.ShouldBeEquivalentTo(paramName);
                }
            }

            public class AddQueryWithNameValueCollection
            {
                [Theory]
                [InlineData("http://example.com", true, typeof(ArgumentNullException), "nameValues")]
                [InlineData(null, false, typeof(ArgumentNullException), "url")]
                public void ShouldThrowArgumentException(string urlString, bool nameValuesIsNull, Type exceptionType, string paramName)
                {
                    // Arrange
                    var url = string.IsNullOrEmpty(urlString) ? default(Uri) : new Uri(urlString);
                    var nameValues = nameValuesIsNull ? default(NameValueCollection) : new NameValueCollection();

                    // Act
                    var sut = new UriQueryAppender();
                    Action activity = () => sut.AppendQuery(url, nameValues);

                    // Assert
                    var assertion = activity.ShouldThrow<ArgumentException>();
                    assertion.And.Should().BeOfType(exceptionType);
                    assertion.And.ParamName.ShouldBeEquivalentTo(paramName);
                }
            }
        }
    }
}
