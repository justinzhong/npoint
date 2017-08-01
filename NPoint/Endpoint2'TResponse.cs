using System;
using System.Linq;
using System.Collections.Generic;
using NPoint.Filters;
using NPoint.Serialization;
using NPoint.Transport;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NPoint
{

    public class Endpoint2<TResponse>
        where TResponse : class
    {
        private IRequestFilterRegistry FilterRegistry { get; }
        private IHttpClientFactory HttpClientFactory { get; }
        private IHttpRequestBuilderFactory RequestBuilderFactory { get; }
        private IJsonSerializer Serializer { get; }
        private Action<HttpResponseMessage> ResponseValidator { get; set; }
        private int Timeout { get; }

        public Endpoint() : this(new DefaultFilterRegistry(), new HttpClientFactory(), new HttpRequestBuilderFactory(), new JsonNetJsonSerializer(), Endpoint2.DefaultTimeout) { }

        public Endpoint(IRequestFilterRegistry filterRegistry, IHttpClientFactory httpClientFactory, IHttpRequestBuilderFactory requestBuilderFactory, IJsonSerializer serializer, int timeout)
        {
            if (filterRegistry == null) throw new ArgumentNullException(nameof(filterRegistry));
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));
            if (requestBuilderFactory == null) throw new ArgumentNullException(nameof(requestBuilderFactory));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            if (timeout <= 0) throw new ArgumentOutOfRangeException("Timeout must be a positive value", nameof(timeout));

            FilterRegistry = filterRegistry;
            HttpClientFactory = httpClientFactory;
            RequestBuilderFactory = requestBuilderFactory;
            Serializer = serializer;
            Timeout = timeout;
        }

        public async Task<string> DispatchRequest(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            // By default, validation and parser are pass-throughs.
            var response = await DispatchRequest(request, Timeout, _ => { }, r => r);

            return response;
        }

        public async Task<string> DispatchRequest(Action<IHttpRequestBuilder> requestSpec)
        {
            if (requestSpec == null) throw new ArgumentNullException(nameof(requestSpec));

            var requestBuilder = RequestBuilderFactory.Create(); // Always use clean nappies
            requestSpec(requestBuilder);

            var response = await DispatchRequest(requestBuilder.Request, Timeout);

            return response;
        }

        public async Task<string> DispatchRequest(Action<IHttpRequestBuilder> requestSpec, int timeout)
        {
            throw new NotImplementedException();
        }

        public async Task<TResponse> DispatchRequest<TResponse>(Action<IHttpRequestBuilder> requestSpec, int timeout, Action<HttpResponseMessage> responseValidator, Func<string, TResponse> parser)
            where TResponse : class
        {
            if (requestSpec == null) throw new ArgumentNullException(nameof(requestSpec));
            if (timeout <= 0) throw new ArgumentOutOfRangeException("Timeout must be a positive value", nameof(timeout));
            if (responseValidator == null) throw new ArgumentNullException(nameof(responseValidator));
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            var requestBuilder = RequestBuilderFactory.Create();
            requestSpec(requestBuilder);

            var response = await DispatchRequest(requestBuilder.Request, timeout, responseValidator, parser);

            return response;
        }

        public async Task<TResponse> DispatchRequest<TResponse>(HttpRequestMessage request, int timeout, Action<HttpResponseMessage> responseValidator, Func<string, TResponse> parser)
            where TResponse : class
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (timeout <= 0) throw new ArgumentOutOfRangeException("Timeout must be a positive value", nameof(timeout));
            if (responseValidator == null) throw new ArgumentNullException(nameof(responseValidator));
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            ApplyFilers(request);

            var cancellationSource = new CancellationTokenSource();

            try
            {
                using (var client = HttpClientFactory.Create())
                {
                    client.Timeout = TimeSpan.FromSeconds(timeout);

                    var responseString = await ReadResponse(client, request, cancellationSource.Token, responseValidator);
                    var response = parser(responseString);

                    return response;
                }
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cancellationSource.Token)
                {
                    throw;
                }

                var timeOutException = new TimeoutException();
                timeOutException.Data.Add(nameof(request), request);
                timeOutException.Data.Add(nameof(timeout), timeout);

                throw timeOutException;
            }
        }

        private void ApplyFilers(HttpRequestMessage request)
        {
            FilterRegistry.Filters.ForEach(filter => filter().Filter(request));
        }

        private async Task<string> ReadResponse(HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken, Action<HttpResponseMessage> responseValidator)
        {
            var response = await client.SendAsync(request, cancellationToken);
            responseValidator(response);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }

}