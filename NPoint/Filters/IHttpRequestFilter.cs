using System.Net.Http;

namespace NPoint.Filters
{
    public interface IHttpRequestFilter
    {
        void Filter(HttpRequestMessage request);
    }
}