using NPoint.Serialization;

namespace NPoint.Config
{
    public interface IApiConfig
    {
        IResponseConverter ResponseConverter { get; }
    }
}