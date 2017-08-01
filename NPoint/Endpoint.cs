using NPoint.Transport;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NPoint
{
    public class Endpoint : IEndpoint
    {
        public static readonly int DefaultTimeout = 60;

        private IHttpRequestBuilderFactory RequestBuilderFactory { get; }
        private IHttpRequestDispatcher RequestDispatcher { get; }
        private EndpointParameter Parameter { get; }

        public Endpoint() : this(new HttpRequestBuilderFactory(), new HttpRequestDispatcher(), new EndpointParameter { Timeout = DefaultTimeout }) { }

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
            var response = await RequestDispatcher.Dispatch(BuildRequest(Parameter.RequestSpecs), Parameter.Timeout);
            var responseBody = await ReadResponse(response);

            return responseBody;
        }

        public async Task<TResponse> Call<TResponse>(Func<string, TResponse> converter)
            where TResponse : class
        {
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            var responseBody = await Call();

            return converter(responseBody);
        }

        public IEndpoint Delete(Uri url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            return AppendSpec(builder => builder.SetEndpoint(url)
                .SetHttpMethod(HttpMethod.Delete));
        }

        public IEndpoint Delete(Uri url, Action<HttpRequestHeaders> headersSpec)
        {
            if (headersSpec == null) throw new ArgumentNullException(nameof(headersSpec));

            return AppendSpec(builder => builder.SetEndpoint(url)
                .SetHttpMethod(HttpMethod.Delete)
                .SetHeaders(headersSpec));
        }

        public IEndpoint Get(Uri url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            return AppendSpec(builder => builder.SetEndpoint(url)
                .SetHttpMethod(HttpMethod.Get));
        }

        public IEndpoint Get(Uri url, Action<HttpRequestHeaders> headersSpec)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (headersSpec == null) throw new ArgumentNullException(nameof(headersSpec));

            return AppendSpec(builder => builder.SetEndpoint(url)
                .SetHttpMethod(HttpMethod.Get)
                .SetHeaders(headersSpec));
        }

        public async Task<HttpResponseMessage> Head(Uri url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            var requestSpecs = new List<Action<IHttpRequestBuilder>>
            {
                builder => builder.SetEndpoint(url)
                    .SetHttpMethod(HttpMethod.Head)
            };
            var request = BuildRequest(requestSpecs);
            var response = await RequestDispatcher.Dispatch(request, Parameter.Timeout);

            return response;
        }

        public async Task<HttpResponseMessage> Head(Uri url, Action<HttpRequestHeaders> headers)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            var requestSpecs = new List<Action<IHttpRequestBuilder>>
            {
                builder => builder.SetEndpoint(url)
                    .SetHttpMethod(HttpMethod.Head)
                    .SetHeaders(headers)
            };
            var request = BuildRequest(requestSpecs);
            var response = await RequestDispatcher.Dispatch(request, Parameter.Timeout);

            return response;
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

        public IEndpoint Post(Uri url, string body, string contentType, Action<HttpRequestHeaders> headersSpec)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(body)) throw new ArgumentException("String cannot be empty or null", nameof(body));
            if (string.IsNullOrEmpty(contentType)) throw new ArgumentException("String cannot be empty or null", nameof(contentType));
            if (headersSpec == null) throw new ArgumentNullException(nameof(headersSpec));

            return AppendSpec(builder => builder.SetEndpoint(url)
                .SetBody(body, contentType)
                .SetHttpMethod(HttpMethod.Post)
                .SetHeaders(headersSpec));
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

        public IEndpoint Put(Uri url, string body, string contentType, Action<HttpRequestHeaders> headersSpec)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(body)) throw new ArgumentException("String cannot be empty or null", nameof(body));
            if (string.IsNullOrEmpty(contentType)) throw new ArgumentException("String cannot be empty or null", nameof(contentType));
            if (headersSpec == null) throw new ArgumentNullException(nameof(headersSpec));

            return AppendSpec(builder => builder.SetEndpoint(url)
                .SetBody(body, contentType)
                .SetHttpMethod(HttpMethod.Put)
                .SetHeaders(headersSpec));
        }

        public IEndpoint OnReceived(Action<HttpResponseMessage> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            return PassOn(new EndpointParameter
            {
                Callback = callback,
                RequestSpecs = Parameter.RequestSpecs,
                Timeout = Parameter.Timeout
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

            return PassOn(new EndpointParameter
            {
                Callback = Parameter.Callback,
                RequestSpecs = Parameter.RequestSpecs,
                Timeout = timeout
            });
        }

        private IEndpoint AppendSpec(Action<IHttpRequestBuilder> requestSpec)
        {
            var appendedSpecs = new List<Action<IHttpRequestBuilder>>(Parameter.RequestSpecs);
            appendedSpecs.Add(requestSpec);

            return PassOn(new EndpointParameter
            {
                Callback = Parameter.Callback,
                RequestSpecs = appendedSpecs,
                Timeout = Parameter.Timeout
            });
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
            Parameter.Callback?.Invoke(response);

            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}
