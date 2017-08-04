using NPoint.Transport;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NPoint
{
    public interface IEndpoint
    {
        Task<string> Call();

        Task<TResponse> Call<TResponse>(Func<HttpResponseMessage, TResponse> converter) where TResponse : class;

        IEndpoint Delete(Uri url);

        IEndpoint Delete(Uri url, Action<HttpRequestHeaders> headersSpec);

        IEndpoint Get(Uri url);

        IEndpoint Get(Uri url, Action<HttpRequestHeaders> headersSpec);

        Task<HttpResponseMessage> Head(Uri url);

        Task<HttpResponseMessage> Head(Uri url, Action<HttpRequestHeaders> headersSpec);

        IEndpoint Post(Uri url, string body, string contentType);

        IEndpoint Post(Uri url, string body, string contentType, Action<HttpRequestHeaders> headersSpec);

        IEndpoint Put(Uri url, string body, string contentType);

        IEndpoint Put(Uri url, string body, string contentType, Action<HttpRequestHeaders> headersSpec);

        IEndpoint OnReceived(Action<HttpResponseMessage> callback);

        IEndpoint RequestWith(HttpRequestMessage request);

        IEndpoint RequestWith(Action<IHttpRequestBuilder> requestSpec);

        IEndpoint TimeoutWhen(int timeout);
    }
}