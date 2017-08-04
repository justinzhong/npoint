using NPoint.Filters;

namespace NPoint
{
    public interface IFilterHttpRequestConfig : IApiConfig
    {
        IHttpRequestFilter HttpRequestFilter { get; }
    }
}