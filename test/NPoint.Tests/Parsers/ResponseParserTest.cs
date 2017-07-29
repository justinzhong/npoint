using FluentAssertions;
using NPoint.Config;
using NPoint.Parsers;
using NPoint.Serialization;
using NPoint.Tests.Data;
using NPoint.Validators;
using NSubstitute;
using Ploeh.AutoFixture;
using System;
using Xunit;

namespace NPoint.Tests.Transport
{
    public class ResponseParserTest
    {
        [Theory]
        [InlineData(true, true, false, "validatorRegistry")]
        [InlineData(true, false, true, "jsonSerializer")]
        [InlineData(false, true, true, "config")]
        public void ShouldThrowExceptionWhenNullIsPassedToCtor(bool configSpecified, bool jsonSerializerSpecified, bool responseValidatorSpecified, string argName)
        {
            // Arrange
            var config = configSpecified ? Substitute.For<INPointConfig>() : default(INPointConfig);
            var validatorRegistry = responseValidatorSpecified ? Substitute.For<IValidatorRegistry>() : default(IValidatorRegistry);
            var jsonSerializer = jsonSerializerSpecified ? Substitute.For<IJsonSerializer>() : default(IJsonSerializer);

            // Act
            Action activity = () => new ResponseParser(config, jsonSerializer, validatorRegistry);

            // Assert
            activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(argName);
        }

        [Theory, AutoNSubstituteData]
        public void ShouldThrowExceptionWhenNullIsPassedToParseResponse(INPointConfig config, IJsonSerializer jsonSerializer, IValidatorRegistry validatorRegistry)
        {
            // Arrange
            var response = string.Empty;

            // Act
            var sut = new ResponseParser(config, jsonSerializer, validatorRegistry);
            Action activity = () => ((IResponseParser)sut).ParseResponse<CustomResponseModel>(response);

            // Assert
            var assertion = activity.ShouldThrowExactly<ArgumentException>();
            assertion.And.Message.StartsWith("String cannot be empty or null");
            assertion.And.ParamName.ShouldBeEquivalentTo(nameof(response));
        }

        [Theory, AutoNSubstituteData]
        public void ShouldSkipValidation(INPointConfig config, IJsonSerializer jsonSerializer, IValidatorRegistry validatorRegistry)
        {
            // Arrange
            var fixture = new Fixture();
            var response = fixture.Create<string>();
            var expectedResponse = fixture.Create<CustomResponseModel>();
            jsonSerializer.Deserialize<CustomResponseModel>(response).Returns(expectedResponse);
            validatorRegistry.HasValidatorFor<CustomResponseModel>().Returns(false);

            // Act
            var sut = new ResponseParser(config, jsonSerializer, validatorRegistry);
            var actual = ((IResponseParser)sut).ParseResponse<CustomResponseModel>(response);

            // Assert
            actual.ShouldBeEquivalentTo(expectedResponse);
            jsonSerializer.Received(1).Deserialize<CustomResponseModel>(response);
            validatorRegistry.Received(1).HasValidatorFor<CustomResponseModel>();
            validatorRegistry.DidNotReceive().GetValidator<CustomResponseModel>();
        }

        [Theory, AutoNSubstituteData]
        public void ShouldThrowCustomValidationException(INPointConfig config, IJsonSerializer jsonSerializer, IValidatorRegistry validatorRegistry)
        {
            // Arrange
            var fixture = new Fixture();
            var response = fixture.Create<string>();
            var expectedResponse = fixture.Create<CustomResponseModel>();
            var validator = Substitute.For<IValidator<CustomResponseModel>>();
            jsonSerializer.Deserialize<CustomResponseModel>(response).Returns(expectedResponse);
            validatorRegistry.HasValidatorFor<CustomResponseModel>().Returns(true);
            validatorRegistry.GetValidator<CustomResponseModel>().Returns(validator);
            validator.When(v => v.Validate(expectedResponse)).Do(_ => { throw new CustomValidationException(); });

            // Act
            var sut = new ResponseParser(config, jsonSerializer, validatorRegistry);
            Action activity = () => ((IResponseParser)sut).ParseResponse<CustomResponseModel>(response);

            // Assert
            activity.ShouldThrowExactly<CustomValidationException>();
            jsonSerializer.Received(1).Deserialize<CustomResponseModel>(response);
            validatorRegistry.Received(1).HasValidatorFor<CustomResponseModel>();
            validatorRegistry.Received(1).GetValidator<CustomResponseModel>();
            validator.Received(1).Validate(expectedResponse);
        }

        class CustomValidationException : Exception
        {
            public override string Message => nameof(CustomValidationException);
        }
    }
}
