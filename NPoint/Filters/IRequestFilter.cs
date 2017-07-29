using System.Net.Http;

namespace NPoint.Filters
{
    public interface IRequestFilter
    {
        void Filter(HttpRequestMessage request);
    }
}