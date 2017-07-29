namespace NPoint.Parsers
{
    public interface IResponseParser
    {
        TResponse ParseResponse<TResponse>(string response) where TResponse : class;
    }
}