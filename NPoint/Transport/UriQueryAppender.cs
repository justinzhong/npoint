using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace NPoint.Transport
{
    public class UriQueryAppender : IUriQueryAppender
    {
        public Uri AppendQuery(Uri url, string name, string value)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("String cannot be empty or null", nameof(name));
            if (string.IsNullOrEmpty(value)) throw new ArgumentException("String cannot be empty or null", nameof(value));

            return AppendQuery(url, new NameValueCollection { { name, value } });
        }

        public Uri AppendQuery(Uri url, NameValueCollection nameValues)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (nameValues == null) throw new ArgumentNullException(nameof(nameValues));
            if (nameValues.Count == 0) return url;

            // Uri.ToString() behaves differently depending on whether the URL
            // is absolute or relative.
            //
            // When the URL is absolute, the ToString() method will unescape
            // some characters in the query part, such as ':', ')' while
            // leave others escaped, such as '?', '&'.
            //
            // When the URL is relative, the ToString() method retains the
            // escaped state.
            //
            // Uri.OriginalString behaves consistently between both absolute
            // and relative URLs.
            var originalUrl = url.OriginalString;
            var query = ExtractQuery(originalUrl);
            var queryBuilder = new StringBuilder(query);

            queryBuilder.Append(string.IsNullOrEmpty(query) ? "?" : "&");

            foreach (var name in nameValues.AllKeys)
            {
                queryBuilder.Append($"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(nameValues[name])}&");
            }

            if (queryBuilder.Length > 0) queryBuilder.Length--;

            var left = ExtractLeftOfQueryAndFragment(originalUrl);
            var right = ExtractFragment(originalUrl);
            var appendedUrl = string.Concat(left, queryBuilder.ToString(), right);

            return new Uri(appendedUrl, UriKind.RelativeOrAbsolute);
        }

        private string ExtractQuery(string url)
        {
            var query = ExtractComponent(url, "?", ExtractRight);
            var fragment = ExtractFragment(query);

            if (string.IsNullOrEmpty(fragment)) return query;

            return query.Substring(0, query.Length - fragment.Length);
        }

        private string ExtractFragment(string url)
        {
            return ExtractComponent(url, "#", ExtractRight);
        }

        private string ExtractLeftOfQueryAndFragment(string url)
        {
            // https://stackoverflow.com/questions/12682952/proper-url-forming-with-query-string-and-anchor-hashtag
            var urlString = new[] { "?", "#" }.Select(delimiter => ExtractComponent(url, delimiter, ExtractLeft)).FirstOrDefault();

            return string.IsNullOrEmpty(urlString) ? url.ToString() : urlString;
        }

        private string ExtractComponent(string url, string delimiter, Func<string, int, string> extractor)
        {
            if (url.Contains(delimiter)) return extractor(url, url.IndexOf(delimiter));

            return string.Empty;
        }

        private string ExtractLeft(string urlString, int index)
        {
            return urlString.Substring(0, index);
        }

        private string ExtractRight(string urlString, int index)
        {
            return urlString.Substring(index);
        }
    }
}
