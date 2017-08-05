using FluentAssertions;
using Xunit;

namespace NPoint.Tests
{
    public class EndpointParameterTest
    {
        [Fact]
        public void ShouldInitialiseToDefaultTimeout()
        {
            // Arrange
            var expected = EndpointParameter.DefaultTimeout;

            // Act
            var sut = new EndpointParameter();
            var actual = sut.Timeout;

            // Arrange
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
