using NPoint.Transport;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NPoint
{
    public static class Endpoint2
    {
        public static int DefaultTimeout => 60;

        public static async Task<string> Get(Uri url)
        {
            var endpoint = new Endpoint<string>();
            var response = await endpoint.DispatchRequest(builder => builder.SetEndpoint(url), DefaultTimeout);

            return response;
        }

        public static async Task<string> Post(Uri url, string payload)
        {
            var endpoint = new Endpoint<string>();
            var response = await endpoint.DispatchRequest(
                builder => builder.SetEndpoint(url)
                .SetHttpMethod(HttpMethod.Post)
                .SetBody(payload), DefaultTimeout);

            return response;
        }

        public static async Task<TResponse> Get<TResponse>(Uri url)
            where TResponse : class
        {
            var endpoint = new Endpoint<TResponse>();
            var response = await endpoint.DispatchRequest<TResponse>(builder => builder.SetEndpoint(url), DefaultTimeout);

            return response;
        }

        public static async Task<TResponse> Post<TResponse>(Uri url, string payload)
            where TResponse : class
        {
            var endpoint = new Endpoint<TResponse>();
            var response = await endpoint.DispatchRequest<TResponse>(
                builder => builder.SetEndpoint(url)
                .SetHttpMethod(HttpMethod.Post)
                .SetBody(payload), DefaultTimeout);

            return response;
        }

        public static async Task<TResponse> Call<TResponse>(Action<IHttpRequestBuilder> requestSpec, int timeout)
            where TResponse : class
        {
            var endpoint = new Endpoint<TResponse>();
            var response = await endpoint.DispatchRequest<TResponse>(requestSpec, timeout);

            return response;
        }
    }
}