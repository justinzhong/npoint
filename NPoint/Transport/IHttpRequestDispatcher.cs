using System.Net.Http;
using System.Threading.Tasks;

namespace NPoint.Transport
{
    public interface IHttpRequestDispatcher
    {
        Task<string> DispatchRequest(HttpRequestMessage request, int timeout);
    }
}