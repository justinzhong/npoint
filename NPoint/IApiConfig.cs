using NPoint.Serialization;

namespace NPoint
{
    public interface IApiConfig
    {
        IResponseConverter ResponseConverter { get; }
    }
}