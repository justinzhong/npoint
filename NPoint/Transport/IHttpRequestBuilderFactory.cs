namespace NPoint.Transport
{
    public interface IHttpRequestBuilderFactory
    {
        IHttpRequestBuilder Create();
    }
}