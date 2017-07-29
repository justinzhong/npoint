using FluentAssertions;
using NPoint.Tests.Data;
using NPoint.Validators;
using Xunit;

namespace NPoint.Tests.Validators
{
    public class DisabledValidatorRegistryTest
    {
        [Fact]
        public void ShouldReturnNegativelyWhenQueryingValidator()
        {
            // Arrange
            // Act
            var sut = (IValidatorRegistry)new DisabledValidatorRegistry();
            var actualResponse = sut.HasValidatorFor<CustomResponseModel>();

            // Assert
            actualResponse.Should().Be(false);
        }
    }
}
