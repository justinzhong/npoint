using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace NPoint.Tests
{
    public class EndpointParameterTest
    {
        [Fact]
        public void ShouldNotAcceptNullEndpointParameter()
        {
            // Arrange
            var endpointParameter = default(EndpointParameter);

            // Act
            Action activity = () => new EndpointParameter(endpointParameter);

            // Assert
            activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("parameter");
        }

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

        [Fact]
        public void ShouldInitialiseRequestSpecs()
        {
            // Arrange
            // Act
            var sut = new EndpointParameter();
            var actual = sut.RequestSpecs;

            // Assert
            actual.Should().NotBeNull().And.HaveCount(0);
        }

        [Theory, NPointData(true)]
        public void ShouldCopyAllProperties(Uri expectedUrl, int expectedTimeout)
        {
            // Arrange
            var expectedCallbacks = new[] { "OnRequestReady", "OnResponseReceived", "OnResponseConverted" };
            var expectedHeaderAccept = "application/json";
            var actualCallbacks = new List<string>();
            var expectedParameter = new EndpointParameter
            {
                OnRequestReady = _ => actualCallbacks.Add("OnRequestReady"),
                OnResponseReceived = _ => actualCallbacks.Add("OnResponseReceived"),
                OnResponseConverted = _ => actualCallbacks.Add("OnResponseConverted"),
                Timeout = expectedTimeout
            };
            expectedParameter.RequestSpecs.Add(builder => builder.SetUrl(expectedUrl));
            expectedParameter.RequestSpecs.Add(builder => builder.SetAccept(expectedHeaderAccept));

            // Act
            var sut = new EndpointParameter(expectedParameter);
            sut.OnRequestReady(Arg.Any<HttpRequestMessage>());
            sut.OnResponseReceived(Arg.Any<HttpResponseMessage>());
            sut.OnResponseConverted(Arg.Any<object>());

            // Assert
            sut.ShouldBeEquivalentTo(expectedParameter);
            actualCallbacks.ShouldBeEquivalentTo(expectedCallbacks);            
        }
    }
}
