using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace NPoint.Tests
{
    public class NPointDataAttribute : AutoDataAttribute
    {
        public NPointDataAttribute() : this(false) { }

        public NPointDataAttribute(bool mockable)
            : base(new Fixture().Customize(new NPointCustomizations(mockable)))
        {
        }
    }
}