using System.Linq;
using NPoint.Transport;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NPoint
{
    public class Endpoint : IEndpoint
    {
        private IHttpRequestBuilderFactory RequestBuilderFactory { get; }
        private IHttpRequestDispatcher RequestDispatcher { get; }
        private EndpointParameter Parameter { get; }

        public Endpoint() : this(new HttpRequestBuilderFactory(), new HttpRequestDispatcher()) { }

        public Endpoint(IHttpRequestBuilderFactory requestBuilderFactory, IHttpRequestDispatcher requestDispatcher) : this(requestBuilderFactory, requestDispatcher, new EndpointParameter()) { }

        public Endpoint(IHttpRequestBuilderFactory requestBuilderFactory, IHttpRequestDispatcher requestDispatcher, EndpointParameter parameter)
        {
            if (requestBuilderFactory == null) throw new ArgumentNullException(nameof(requestBuilderFactory));
            if (requestDispatcher == null) throw new ArgumentNullException(nameof(requestDispatcher));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            RequestBuilderFactory = requestBuilderFactory;
            RequestDispatcher = requestDispatcher;
            Parameter = parameter;
        }

        public async Task<string> Call()
        {
            var request = BuildRequest(Parameter.RequestSpecs);
            var response = await RequestDispatcher.Dispatch(request, Parameter.Timeout);
            var responseBody = await ReadResponse(response);

            return responseBody;
        }

        public async Task<TResponse> Call<TResponse>(Func<HttpResponseMessage, Task<TResponse>> converter)
            where TResponse : class
        {
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            var request = BuildRequest(Parameter.RequestSpecs);
            var response = await RequestDispatcher.Dispatch(request, Parameter.Timeout);
            var convertedResponse = await ReadResponse(response, converter);

            return convertedResponse;
        }

        public async Task<HttpResponseMessage> CallThrough()
        {
            var request = BuildRequest(Parameter.RequestSpecs);
            var response = await RequestDispatcher.Dispatch(request, Parameter.Timeout);

            return response;
        }

        public IEndpoint Delete(Uri url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            return AppendSpec(builder => builder.SetEndpoint(url)
                .SetHttpMethod(HttpMethod.Delete));
        }

        public IEndpoint Get(Uri url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            return AppendSpec(builder => builder.SetEndpoint(url)
                .SetHttpMethod(HttpMethod.Get));
        }

        public IEndpoint Head(Uri url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            return AppendSpec(builder => builder.SetEndpoint(url)
                .SetHttpMethod(HttpMethod.Head));
        }

        public IEndpoint Post(Uri url, string body, string contentType)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(body)) throw new ArgumentException("String cannot be empty or null", nameof(body));
            if (string.IsNullOrEmpty(contentType)) throw new ArgumentException("String cannot be empty or null", nameof(contentType));

            return AppendSpec(builder => builder.SetEndpoint(url)
                .SetBody(body, contentType)
                .SetHttpMethod(HttpMethod.Post));
        }

        public IEndpoint Put(Uri url, string body, string contentType)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(body)) throw new ArgumentException("String cannot be empty or null", nameof(body));
            if (string.IsNullOrEmpty(contentType)) throw new ArgumentException("String cannot be empty or null", nameof(contentType));

            return AppendSpec(builder => builder.SetEndpoint(url)
                .SetBody(body, contentType)
                .SetHttpMethod(HttpMethod.Put));
        }

        public IEndpoint OnReceived(Action<HttpResponseMessage> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            return PassOn(new EndpointParameter(Parameter)
            {
                OnResponseReceived = callback,
            });
        }

        public IEndpoint RequestWith(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return AppendSpec(builder => builder.SetRequest(request));
        }

        public IEndpoint RequestWith(Action<IHttpRequestBuilder> requestSpec)
        {
            if (requestSpec == null) throw new ArgumentNullException(nameof(requestSpec));

            return AppendSpec(requestSpec);
        }

        public IEndpoint TimeoutWhen(int timeout)
        {
            if (timeout < 0) throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be a non-negative value");

            return PassOn(new EndpointParameter(Parameter)
            {
                Timeout = timeout
            });
        }

        private IEndpoint AppendSpec(Action<IHttpRequestBuilder> requestSpec)
        {
            var appendedSpecs = new List<Action<IHttpRequestBuilder>>(Parameter.RequestSpecs);
            appendedSpecs.Add(requestSpec);

            var parameter = new EndpointParameter(Parameter);
            parameter.RequestSpecs.Add(requestSpec);

            return PassOn(parameter);
        }

        private IEndpoint PassOn(EndpointParameter parameter)
        {
            return new Endpoint(RequestBuilderFactory, RequestDispatcher, parameter);
        }

        private HttpRequestMessage BuildRequest(IEnumerable<Action<IHttpRequestBuilder>> requestSpecs)
        {
            if (requestSpecs == null) throw new InvalidOperationException("No request specified");

            var requestBuilder = RequestBuilderFactory.Create();

            foreach (var spec in requestSpecs) spec(requestBuilder);

            return requestBuilder.Build();
        }

        private async Task<string> ReadResponse(HttpResponseMessage response)
        {
            Parameter.OnResponseReceived?.Invoke(response);

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        private async Task<TResponse> ReadResponse<TResponse>(HttpResponseMessage response, Func<HttpResponseMessage, Task<TResponse>> converter)
        {
            Parameter.OnResponseReceived?.Invoke(response);

            var convertedResponse = await converter(response);

            return convertedResponse;
        }
    }
}
