using NPoint.Transport;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NPoint
{
    public interface IEndpoint
    {
        Task<string> Invoke(HttpRequestMessage request);

        Task<string> Invoke(Action<IHttpRequestBuilder> requestSpec);

        Task<TResponse> Invoke<TResponse>(HttpRequestMessage request) where TResponse : class;

        Task<TResponse> Invoke<TResponse>(Action<IHttpRequestBuilder> requestSpec) where TResponse : class;
    }
}