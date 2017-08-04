namespace NPoint.Serialization
{
    public interface IResponseConverter
    {
        TResponse Convert<TResponse>(string responseString) where TResponse : class;
    }
}