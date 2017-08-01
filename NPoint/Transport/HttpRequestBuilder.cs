using NPoint.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace NPoint.Transport
{
    public class HttpRequestBuilder : IHttpRequestBuilder
    {
        private IEnumerable<Action<HttpRequestMessage>> BuildSpecs { get; }
        private IUriQueryAppender QueryAppender { get; }
        private IJsonSerializer Serializer { get; }

        public HttpRequestBuilder(IUriQueryAppender queryAppender, IJsonSerializer serializer) : this(queryAppender, serializer, new List<Action<HttpRequestMessage>>()) { }

        public HttpRequestBuilder(IUriQueryAppender queryAppender, IJsonSerializer serializer, IEnumerable<Action<HttpRequestMessage>> buildSpecs)
        {
            if (queryAppender == null) throw new ArgumentNullException(nameof(queryAppender));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            if (buildSpecs == null) throw new ArgumentNullException(nameof(buildSpecs));

            BuildSpecs = buildSpecs;
            QueryAppender = queryAppender;
            Serializer = serializer;
        }

        public HttpRequestBuilder AddQuery(string name, string value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("String cannot be null or empty", nameof(name));
            if (string.IsNullOrEmpty(value)) throw new ArgumentException("String cannot be null or empty", nameof(value));

            return AppendSpec(request => request.RequestUri = QueryAppender.AppendQuery(request.RequestUri, name, value));
        }

        public HttpRequestBuilder AddQuery(NameValueCollection nameValues)
        {
            if (nameValues == null) throw new ArgumentNullException(nameof(nameValues));

            return AppendSpec(request => request.RequestUri = QueryAppender.AppendQuery(request.RequestUri, nameValues));
        }

        public HttpRequestBuilder SetAccept(string accept)
        {
            if (string.IsNullOrEmpty(accept)) throw new ArgumentException("String cannot be null or empty", nameof(accept));

            return AppendSpec(request => request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept)));
        }

        private HttpRequestBuilder AppendSpec(Action<HttpRequestMessage> spec)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));

            var specs = new List<Action< HttpRequestMessage>>(BuildSpecs);
            specs.Add(spec);

            return new HttpRequestBuilder(QueryAppender, Serializer, specs);
        }

        public HttpRequestBuilder SetBody(string body, string contentType = null)
        {
            if (string.IsNullOrEmpty(body)) throw new ArgumentException("String cannot be null or empty", nameof(body));

            Request.Content = BuildMessageContent(body, contentType);

            return this;
        }

        public HttpRequestBuilder SetEndpoint(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentException("String cannot be null or empty", nameof(url));

            Request.RequestUri = new Uri(url);

            return this;
        }

        public HttpRequestBuilder SetEndpoint(Uri url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            Request.RequestUri = url;

            return this;
        }

        public HttpRequestBuilder SetHeaders(Action<HttpRequestHeaders> headersSpec)
        {
            if (headersSpec == null) throw new ArgumentNullException(nameof(headersSpec));

            headersSpec(Request.Headers);

            return this;
        }

        public HttpRequestBuilder SetHttpMethod(HttpMethod method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            Request.Method = method;

            return this;
        }

        public HttpRequestBuilder SetJson<TPayload>(TPayload payload, string contentType = "application/json")
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));

            var body = Serializer.Serialize(payload);

            SetBody(body, contentType);

            return this;
        }

        public HttpRequestBuilder SetRequest(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            Request = request;

            return this;
        }

        private HttpContent BuildMessageContent(string body, string contentType)
        {
            if (string.IsNullOrEmpty(body)) return null;

            var content = new StringContent(body, Encoding.UTF8);

            if (!string.IsNullOrEmpty(contentType))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Headers.ContentType.CharSet = Encoding.UTF8.WebName;
            }

            return content;
        }
    }
}
