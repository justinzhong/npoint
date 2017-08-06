using Ploeh.AutoFixture;

namespace NPoint.Tests
{
    public class EndpointParameterCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<EndpointParameter>(composer =>
                composer
                    .With(parameter => parameter.Timeout, fixture.Create<int>()));
        }
    }
}