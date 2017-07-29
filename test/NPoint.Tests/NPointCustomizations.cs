using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using System.Collections.Generic;

namespace NPoint.Tests
{
    public class NPointCustomizations : CompositeCustomization
    {
        public NPointCustomizations() : this(false) { }

        public NPointCustomizations(bool mockable) : base(AssembleCustomizations(mockable)) { }

        private static IEnumerable<ICustomization> AssembleCustomizations(bool mockable)
        {
            yield return new HttpMessageRequestCustomization();

            if (mockable) yield return new AutoNSubstituteCustomization();
        }
    }
}