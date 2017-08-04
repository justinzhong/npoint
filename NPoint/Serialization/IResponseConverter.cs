using System.Net.Http;

namespace NPoint.Serialization
{
    public interface IResponseConverter
    {
        TResponse Convert<TResponse>(HttpResponseMessage response) where TResponse : class;
    }
}