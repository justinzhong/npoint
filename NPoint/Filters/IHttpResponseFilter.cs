using System.Net.Http;

namespace NPoint.Filters
{
    public interface IHttpResponseFilter
    {
        void Filter(HttpResponseMessage response);
    }
}