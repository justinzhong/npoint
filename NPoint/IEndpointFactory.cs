namespace NPoint
{
    public interface IEndpointFactory
    {
        IEndpoint Create(EndpointParameter parameter);
    }
}