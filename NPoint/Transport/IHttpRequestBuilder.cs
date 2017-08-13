using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NPoint.Transport
{
    public interface IHttpRequestBuilder
    {
        IHttpRequestBuilder AddQuery(NameValueCollection nameValues);
        IHttpRequestBuilder AddQuery(string name, string value);
        HttpRequestMessage Build();
        IHttpRequestBuilder SetAccept(string accept);
        IHttpRequestBuilder SetBody(string body, string contentType);
        IHttpRequestBuilder SetHeader(Action<HttpRequestHeaders> headerSpec);
        IHttpRequestBuilder SetHttpMethod(HttpMethod method);
        IHttpRequestBuilder SetJson<TPayload>(TPayload payload);
        IHttpRequestBuilder SetRequest(HttpRequestMessage request);
        IHttpRequestBuilder SetUrl(Uri url);
    }
}