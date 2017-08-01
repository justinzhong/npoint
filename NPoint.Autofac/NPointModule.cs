using Autofac;
using NPoint.Parsers;
using NPoint.Serialization;
using NPoint.Transport;

namespace NPoint.Autofac
{
    public class NPointModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JsonNetJsonSerializer>().As<IJsonSerializer>();
            builder.RegisterType<ResponseParser>().As<IResponseParser>();
            builder.RegisterType<UriQueryAppender>().As<IUriQueryAppender>();
            builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>();
            builder.RegisterType<Endpoint2>().As<IHttpRequestDispatcher>();
            builder.RegisterType<HttpRequestBuilder>().As<IHttpRequestBuilder>();
            builder.RegisterType<HttpRequestBuilderFactory>().As<IHttpRequestBuilderFactory>();
            builder.RegisterType<Endpoint2>().As<IEndpoint>();
        }
    }
}
