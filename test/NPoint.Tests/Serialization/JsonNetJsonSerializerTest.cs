using FluentAssertions;
using NPoint.Serialization;
using NPoint.Tests.Data;
using System;
using Xunit;

namespace NPoint.Tests.Serialization
{
    public class JsonNetJsonSerializerTest
    {
        [Fact]
        public void ShouldThrowArgumentNullException()
        {
            // Arrange
            var json = string.Empty;

            // Act
            var sut = new JsonNetJsonSerializer();
            Action activity = () => sut.Deserialize<CustomResponseModel>(json);

            // Assert
            var assertion = activity.ShouldThrowExactly<ArgumentException>();
            assertion.And.Message.StartsWith("String cannot be empty or null");
            assertion.And.ParamName.ShouldBeEquivalentTo(nameof(json));
        }

        [Theory, NPointData]
        public void ShouldDeserializeModel(CustomResponseModel expected)
        {
            // Arrange
            var json = $@"{{""Id"":{expected.Id},""Name"":""{expected.Name}""}}";

            // Act
            var sut = new JsonNetJsonSerializer();
            var actual = sut.Deserialize<CustomResponseModel>(json);

            // Assert
            actual.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldDeserializeNullJsonLiteral()
        {
            // Arrange
            var json = "null";
            var expected = default(CustomResponseModel);

            // Act
            var sut = new JsonNetJsonSerializer();
            var actual = sut.Deserialize<CustomResponseModel>(json);

            // Assert
            actual.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldSerializeNullGraph()
        {
            // Arrange
            var model = default(CustomResponseModel);
            var expected = "null";

            // Act
            var sut = new JsonNetJsonSerializer();
            var actual = sut.Serialize(model);

            // Assert
            actual.ShouldBeEquivalentTo(expected);
        }

        [Theory, NPointData]
        public void ShouldSerializeModel(CustomResponseModel model)
        {
            // Arrange
            var expected = $@"{{""Id"":{model.Id},""Name"":""{model.Name}""}}";

            // Act
            var sut = new JsonNetJsonSerializer();
            var actual = sut.Serialize(model);

            // Assert
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
