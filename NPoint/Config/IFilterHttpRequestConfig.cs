using NPoint.Filters;

namespace NPoint.Config
{
    public interface IFilterHttpRequestConfig : IApiConfig
    {
        IHttpRequestFilter HttpRequestFilter { get; }
    }
}