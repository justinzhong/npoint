using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using System.Net.Http;

namespace NPoint.Tests
{
    public class HttpMessageRequestCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new TypeRelay(typeof(HttpContent), typeof(StringContent)));
        }
    }
}