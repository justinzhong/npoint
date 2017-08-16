using NPoint.Filters;

namespace NPoint.Config
{
    public interface IModelFilterConfig : IApiConfig
    {
        IModelFilter ModelFilter { get; }
    }
}