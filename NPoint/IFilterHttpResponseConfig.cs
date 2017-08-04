using NPoint.Filters;

namespace NPoint
{
    public interface IFilterHttpResponseConfig : IApiConfig
    {
        IHttpResponseFilter HttpResponseFilter { get; }
    }
}