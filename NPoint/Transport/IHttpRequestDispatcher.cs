using System.Net.Http;
using System.Threading.Tasks;

namespace NPoint.Transport
{
    public interface IHttpRequestDispatcher
    {
        Task<HttpResponseMessage> Dispatch(HttpRequestMessage request, int timeout);
    }
}