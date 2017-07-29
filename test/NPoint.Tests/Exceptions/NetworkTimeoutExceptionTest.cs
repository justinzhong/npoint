using FluentAssertions;
using NPoint.Exceptions;
using System;
using System.Net.Http;
using Xunit;

namespace NPoint.Tests.Exceptions
{
    public class NetworkTimeoutExceptionTest
    {
        [Fact]
        public void ShouldThrowExceptionWhenNullRequestIsPassedToCtor()
        {
            // Arrange
            var request = default(HttpRequestMessage);

            // Act
            Action activity = () => new NetworkTimeoutException(null, 0);

            // Assert
            activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(nameof(request));
        }

        [Theory, NPointData]
        public void ShouldThrowServerErrorException(int timeout, HttpRequestMessage request)
        {
            // Arrange
            var expectedErrorMessage = $"Did not receive any response from the server after {timeout} seconds";

            // Act
            Action activity = () => { throw new NetworkTimeoutException(request, timeout); };

            // Assert
            activity.ShouldThrowExactly<NetworkTimeoutException>().WithMessage(expectedErrorMessage)
                .And.Request.ShouldBeEquivalentTo(request);
        }
    }
}
