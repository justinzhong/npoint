using NPoint.Serialization;

namespace NPoint.Config
{
    public class DefaultApiConfig : IApiConfig
    {
        public IResponseConverter ResponseConverter => new JsonResponseConverter();
    }
}
