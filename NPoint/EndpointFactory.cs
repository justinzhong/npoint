using System;
using NPoint.Transport;

namespace NPoint
{
    public class EndpointFactory : IEndpointFactory
    {
        private IHttpRequestBuilderFactory RequestBuilderFactory { get; }
        private IHttpRequestDispatcher RequestDispatcher { get; }

        public EndpointFactory() : this(new HttpRequestBuilderFactory(), new HttpRequestDispatcher()) { }

        public EndpointFactory(IHttpRequestBuilderFactory requestBuilderFactory, IHttpRequestDispatcher requestDispatcher)
        {
            if (requestBuilderFactory == null) throw new ArgumentNullException(nameof(requestBuilderFactory));
            if (requestDispatcher == null) throw new ArgumentNullException(nameof(requestDispatcher));

            RequestBuilderFactory = requestBuilderFactory;
            RequestDispatcher = requestDispatcher;
        }

        public IEndpoint Create(EndpointParameter parameter)
        {
            return new Endpoint(RequestBuilderFactory, RequestDispatcher, parameter);
        }
    }
}