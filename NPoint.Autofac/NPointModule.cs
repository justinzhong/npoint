using Autofac;
using NPoint.Serialization;
using NPoint.Transport;

namespace NPoint.Autofac
{
    public class NPointModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JsonNetJsonSerializer>().As<IJsonSerializer>();
            builder.RegisterType<UriQueryAppender>().As<IUriQueryAppender>();
            builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>();
            builder.RegisterType<HttpRequestDispatcher>().As<IHttpRequestDispatcher>();
            builder.RegisterType<HttpRequestBuilder>().As<IHttpRequestBuilder>();
            builder.RegisterType<HttpRequestBuilderFactory>().As<IHttpRequestBuilderFactory>();
            builder.RegisterType<Endpoint>().As<IEndpoint>();
        }
    }
}
