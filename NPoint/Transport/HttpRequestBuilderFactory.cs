using NPoint.Serialization;

namespace NPoint.Transport
{
    public class HttpRequestBuilderFactory : IHttpRequestBuilderFactory
    {
        private IUriQueryAppender QueryAppender { get; }
        private IJsonSerializer Serializer { get; }

        public HttpRequestBuilderFactory() : this(new UriQueryAppender(), new JsonNetJsonSerializer()) { }

        public HttpRequestBuilderFactory(IUriQueryAppender queryAppender, IJsonSerializer serializer)
        {
            QueryAppender = queryAppender;
            Serializer = serializer;
        }

        public IHttpRequestBuilder Create()
        {
            return new HttpRequestBuilder(QueryAppender, Serializer);
        }
    }
}