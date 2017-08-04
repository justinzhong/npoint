using NPoint.Filters;

namespace NPoint.Config
{
    public interface IFilterHttpResponseConfig : IApiConfig
    {
        IHttpResponseFilter HttpResponseFilter { get; }
    }
}