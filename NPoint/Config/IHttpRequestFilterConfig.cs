using NPoint.Filters;

namespace NPoint.Config
{
    public interface IHttpRequestFilterConfig : IApiConfig
    {
        IHttpRequestFilter HttpRequestFilter { get; }
    }
}