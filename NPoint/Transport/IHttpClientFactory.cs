using System.Net.Http;

namespace NPoint.Transport
{
    public interface IHttpClientFactory
    {
        HttpClient Create();
    }
}