using System;
using NPoint.Serialization;

namespace NPoint.Transport
{
    public class HttpRequestBuilderFactory : IHttpRequestBuilderFactory
    {
        private IUriQueryAppender QueryAppender { get; }
        private IJsonSerializer Serializer { get; }

        public HttpRequestBuilderFactory(IUriQueryAppender queryAppender, IJsonSerializer serializer)
        {
            if (queryAppender == null) throw new ArgumentNullException(nameof(queryAppender));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            QueryAppender = queryAppender;
            Serializer = serializer;
        }

        public IHttpRequestBuilder Create()
        {
            return new HttpRequestBuilder(QueryAppender, Serializer);
        }
    }
}