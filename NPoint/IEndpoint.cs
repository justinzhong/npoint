using NPoint.Transport;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NPoint
{
    public interface IEndpoint
    {
        #region HTTP verb methods

        IEndpoint Delete(Uri url);

        IEndpoint Get(Uri url);

        IEndpoint Head(Uri url);

        IEndpoint Post(Uri url, string body, string contentType);

        IEndpoint Put(Uri url, string body, string contentType);

        #endregion

        Task<string> Call();

        Task<TResponse> Call<TResponse>(Func<HttpResponseMessage, Task<TResponse>> converter) where TResponse : class;

        Task<HttpResponseMessage> CallThrough();

        IEndpoint OnReceived(Action<HttpResponseMessage> callback);

        IEndpoint RequestWith(HttpRequestMessage request);

        IEndpoint RequestWith(Action<IHttpRequestBuilder> requestSpec);

        IEndpoint SetHeader(Action<HttpRequestHeaders> headerSpec);

        IEndpoint TimeoutWhen(int timeout);
    }
}