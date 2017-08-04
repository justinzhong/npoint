using NPoint.Filters;

namespace NPoint
{
    public interface IFilterConvertedResponseConfig : IApiConfig
    {
        IResponseFilter ResponseFilter { get; }
    }
}