using Ploeh.AutoFixture;
using System.Net.Http;

namespace NPoint.Tests
{
    public class HttpRequestMessageCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<HttpRequestMessage>(composer =>
                composer.With(p => p.Method, HttpMethod.Get));
            fixture.Customizations.Add(new HttpContentBuilder());
        }
    }
}