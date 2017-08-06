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
        private HttpRequestMessage RequestBaseline { get; }

        public HttpRequestBuilder() : this(new UriQueryAppender(), new JsonNetJsonSerializer()) { }

        public HttpRequestBuilder(IUriQueryAppender queryAppender, IJsonSerializer serializer) :
            this(queryAppender, serializer, new HttpRequestMessage(), new List<Action<HttpRequestMessage>>())
        { }

        public HttpRequestBuilder(
            IUriQueryAppender queryAppender,
            IJsonSerializer serializer,
            HttpRequestMessage requestBaseline,
            IEnumerable<Action<HttpRequestMessage>> buildSpecs)
        {
            if (queryAppender == null) throw new ArgumentNullException(nameof(queryAppender));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            if (requestBaseline == null) throw new ArgumentNullException(nameof(requestBaseline));
            if (buildSpecs == null) throw new ArgumentNullException(nameof(buildSpecs));

            QueryAppender = queryAppender;
            Serializer = serializer;
            RequestBaseline = requestBaseline;
            BuildSpecs = buildSpecs;
        }

        public IHttpRequestBuilder AddQuery(string name, string value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("String cannot be null or empty", nameof(name));
            if (string.IsNullOrEmpty(value)) throw new ArgumentException("String cannot be null or empty", nameof(value));

            return AddQuery(new NameValueCollection { { name, value } });
        }

        public IHttpRequestBuilder AddQuery(NameValueCollection nameValues)
        {
            if (nameValues == null) throw new ArgumentNullException(nameof(nameValues));

            return AppendSpec(request => request.RequestUri = QueryAppender.AppendQuery(request.RequestUri, nameValues));
        }

        public HttpRequestMessage Build()
        {
            foreach (var spec in BuildSpecs) spec(RequestBaseline);

            return RequestBaseline;
        }

        public IHttpRequestBuilder SetAccept(string accept)
        {
            if (string.IsNullOrEmpty(accept)) throw new ArgumentException("String cannot be null or empty", nameof(accept));

            return AppendSpec(request => request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept)));
        }

        public IHttpRequestBuilder SetBody(string body, string contentType)
        {
            if (string.IsNullOrEmpty(body)) throw new ArgumentException("String cannot be null or empty", nameof(body));
            if (string.IsNullOrEmpty(contentType)) throw new ArgumentException("String cannot be null or empty", nameof(contentType));

            return AppendSpec(request => request.Content = BuildMessageContent(body, contentType));
        }

        public IHttpRequestBuilder SetEndpoint(Uri url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            return AppendSpec(request => request.RequestUri = url);
        }

        public IHttpRequestBuilder SetHeaders(Action<HttpRequestHeaders> headersSpec)
        {
            if (headersSpec == null) throw new ArgumentNullException(nameof(headersSpec));

            return AppendSpec(request => headersSpec(request.Headers));
        }

        public IHttpRequestBuilder SetHttpMethod(HttpMethod method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            return AppendSpec(request => request.Method = method);
        }

        public IHttpRequestBuilder SetJson<TPayload>(TPayload payload)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));

            return AppendSpec(request =>
            {
                var body = Serializer.Serialize(payload);
                request.Content = BuildMessageContent(body, "application/json");
            });
        }

        public IHttpRequestBuilder SetRequest(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return new HttpRequestBuilder(QueryAppender, Serializer, request, BuildSpecs);
        }

        private IHttpRequestBuilder AppendSpec(Action<HttpRequestMessage> spec)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));

            var specs = new List<Action<HttpRequestMessage>>(BuildSpecs);
            specs.Add(spec);

            return new HttpRequestBuilder(QueryAppender, Serializer, RequestBaseline, specs);
        }

        private HttpContent BuildMessageContent(string body, string contentType)
        {
            var content = new StringContent(body, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Headers.ContentType.CharSet = Encoding.UTF8.WebName;

            return content;
        }
    }
}
