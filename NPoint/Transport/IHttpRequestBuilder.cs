using System;
using System.Collections.Specialized;
using System.Net.Http;

namespace NPoint.Transport
{
    public interface IHttpRequestBuilder
    {
        HttpRequestMessage Request { get; }

        HttpRequestBuilder AddQuery(NameValueCollection nameValues);
        HttpRequestBuilder AddQuery(string name, string value);
        HttpRequestBuilder SetAccept(string accept);
        HttpRequestBuilder SetBody(string body, string contentType = null);
        HttpRequestBuilder SetEndpoint(Uri url);
        HttpRequestBuilder SetEndpoint(string url);
        HttpRequestBuilder SetHttpMethod(HttpMethod method);
        HttpRequestBuilder SetJson<TPayload>(TPayload payload, string contentType = "application/json");
    }
}