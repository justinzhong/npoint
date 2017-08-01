using Ploeh.AutoFixture;

namespace NPoint.Tests
{
    public class HttpContentBuilderCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new HttpContentBuilder());
        }
    }
}