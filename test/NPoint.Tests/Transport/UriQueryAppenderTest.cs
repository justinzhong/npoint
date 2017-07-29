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
    }
}
