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
            var response = await endpoint.Call(r => Config.ResponseConverter.Convert<TResponse>(r));

            return response;
        }

        private void ApplyFilter<TFilterConfig>(Action<TFilterConfig> filterConfigSpec)
            where TFilterConfig : class, IApiConfig
        {
            var config = Config as TFilterConfig;

            if (config == null) return;

            filterConfigSpec(config);
        }

        private IEndpoint SetupEndpoint<TResponse>(Action<IEndpoint> endpointSpec)
            where TResponse: class
        {
            var parameter = new EndpointParameter();

            ApplyFilter<IFilterHttpRequestConfig>(config => parameter.OnRequestReady = config.HttpRequestFilter.Filter);
            ApplyFilter<IFilterHttpResponseConfig>(config => parameter.OnResponseReceived = config.HttpResponseFilter.Filter);
            ApplyFilter<IFilterConvertedResponseConfig>(config => parameter.OnResponseConverted = config.ResponseFilter.Filter);

            var endpoint = EndpointFactory.Create(parameter);
            endpointSpec(endpoint);

            return endpoint;
        }
    }
}
