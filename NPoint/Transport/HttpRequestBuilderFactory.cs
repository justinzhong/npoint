namespace NPoint.Transport
{
    public class HttpRequestBuilderFactory : IHttpRequestBuilderFactory
    {
        public IHttpRequestBuilder Create()
        {
            return new HttpRequestBuilder();
        }
    }
}