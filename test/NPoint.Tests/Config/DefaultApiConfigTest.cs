using FluentAssertions;
using NPoint.Config;
using NPoint.Serialization;
using Xunit;

namespace NPoint.Tests.Config
{
    public class DefaultApiConfigTest
    {
        [Fact]
        public void ShouldReturnJsonResponseConverter()
        {
            // Arrange
            var expected = typeof(JsonResponseConverter);

            // Act
            var sut = new DefaultApiConfig();
            var converter = sut.ResponseConverter;
            var actual = converter.GetType();

            // Assert
            converter.Should().NotBeNull();
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
