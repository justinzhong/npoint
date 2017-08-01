using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NPoint.Transport
{
    public interface IHttpRequestBuilder
    {
        HttpRequestBuilder AddQuery(NameValueCollection nameValues);
        HttpRequestBuilder AddQuery(string name, string value);
        HttpRequestMessage Build();
        HttpRequestBuilder SetAccept(string accept);
        HttpRequestBuilder SetBody(string body, string contentType);
        HttpRequestBuilder SetEndpoint(Uri url);
        HttpRequestBuilder SetHeaders(Action<HttpRequestHeaders> headersSpec);
        HttpRequestBuilder SetHttpMethod(HttpMethod method);
        HttpRequestBuilder SetJson<TPayload>(TPayload payload);
        HttpRequestBuilder SetRequest(HttpRequestMessage request);
    }
}