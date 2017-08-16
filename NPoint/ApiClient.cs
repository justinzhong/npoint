using NPoint.Config;
using System;
using System.Threading.Tasks;

namespace NPoint
{
    public class ApiClient : IApiClient
    {
        private IApiConfig Config { get; }
        private IEndpointFactory EndpointFactory { get; }

        public ApiClient() : this(new DefaultApiConfig(), new EndpointFactory()) { }

        public ApiClient(IApiConfig config) : this(config, new EndpointFactory()) { }

        public ApiClient(IApiConfig config, IEndpointFactory endpointFactory)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (endpointFactory == null) throw new ArgumentNullException(nameof(endpointFactory));

            Config = config;
            EndpointFactory = endpointFactory;
        }

        public async Task<TResponse> Call<TResponse>(Action<IEndpoint> endpointSpec)
            where TResponse : class
        {
            if (endpointSpec == null) throw new ArgumentNullException(nameof(endpointSpec));

            var endpoint = SetupEndpoint<TResponse>(endpointSpec);
            var response = await endpoint.Call(request => Config.ResponseConverter.Convert<TResponse>(request));

            return response;
        }

        private void ApplyFilter<TFilterConfig>(Action<TFilterConfig> filterConfigSpec)
            where TFilterConfig : class, IApiConfig
        {
            var config = Config as TFilterConfig;

            if (config == null) return;

            filterConfigSpec(config);
        }

        private void ApplyFilters(EndpointParameter parameter)
        {
            ApplyFilter<IHttpRequestFilterConfig>(config => parameter.OnRequestReady = config.HttpRequestFilter.Filter);
            ApplyFilter<IHttpResponseFilterConfig>(config => parameter.OnResponseReceived = config.HttpResponseFilter.Filter);
            ApplyFilter<IModelFilterConfig>(config => parameter.OnResponseConverted = config.ModelFilter.Filter);
        }

        private IEndpoint SetupEndpoint<TResponse>(Action<IEndpoint> endpointSpec)
            where TResponse: class
        {
            var parameter = new EndpointParameter();
            ApplyFilters(parameter);

            var endpoint = EndpointFactory.Create(parameter);
            endpointSpec(endpoint);

            return endpoint;
        }
    }
}
