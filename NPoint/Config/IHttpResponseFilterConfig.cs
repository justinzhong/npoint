using NPoint.Filters;

namespace NPoint.Config
{
    public interface IHttpResponseFilterConfig : IApiConfig
    {
        IHttpResponseFilter HttpResponseFilter { get; }
    }
}