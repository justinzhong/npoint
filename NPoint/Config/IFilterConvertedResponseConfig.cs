using NPoint.Filters;

namespace NPoint.Config
{
    public interface IFilterConvertedResponseConfig : IApiConfig
    {
        IResponseFilter ResponseFilter { get; }
    }
}