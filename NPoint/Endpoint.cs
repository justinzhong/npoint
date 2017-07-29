using NPoint.Config;
using NPoint.Parsers;
using NPoint.Transport;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NPoint
{
    public class Endpoint : IEndpoint
    {
        private INPointConfig Config { get; }
        private IHttpRequestBuilderFactory RequestBuilderFactory { get; }
        private IHttpRequestDispatcher RequestDispatcher { get; }
        private IResponseParser ResponseParser { get; }

        public Endpoint(IHttpRequestBuilderFactory requestBuilderFactory,
            IHttpRequestDispatcher requestDispatcher,
            IResponseParser responseParser) : this(new DefaultNPointConfig(), requestBuilderFactory, requestDispatcher, responseParser) { }

        public Endpoint(INPointConfig config, 
            IHttpRequestBuilderFactory requestBuilderFactory,
            IHttpRequestDispatcher requestDispatcher,
            IResponseParser responseParser)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (requestBuilderFactory == null) throw new ArgumentNullException(nameof(requestBuilderFactory));
            if (requestDispatcher == null) throw new ArgumentNullException(nameof(requestDispatcher));
            if (responseParser == null) throw new ArgumentNullException(nameof(responseParser));

            Config = config;
            RequestBuilderFactory = requestBuilderFactory;
            RequestDispatcher = requestDispatcher;
            ResponseParser = responseParser;
        }

        public async Task<string> Invoke(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var response = await RequestDispatcher.DispatchRequest(request, Config.RequestTimeout);

            return response;
        }

        public async Task<string> Invoke(Action<IHttpRequestBuilder> requestSpec)
        {
            if (requestSpec == null) throw new ArgumentNullException(nameof(requestSpec));

            var requestBuilderInstance = RequestBuilderFactory.Create(); // Always use clean nappies
            requestSpec(requestBuilderInstance);
            var request = requestBuilderInstance.Request;
            var response = await RequestDispatcher.DispatchRequest(request, Config.RequestTimeout);

            return response;
        }

        public async Task<TResponse> Invoke<TResponse>(HttpRequestMessage request)
            where TResponse : class
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var responseString = await Invoke(request);
            var response = ResponseParser.ParseResponse<TResponse>(responseString);

            return response;
        }

        public async Task<TResponse> Invoke<TResponse>(Action<IHttpRequestBuilder> requestSpec)
            where TResponse : class
        {
            if (requestSpec == null) throw new ArgumentNullException(nameof(requestSpec));

            var responseString = await Invoke(requestSpec);
            var response = ResponseParser.ParseResponse<TResponse>(responseString);

            return response;
        }
    }
}
