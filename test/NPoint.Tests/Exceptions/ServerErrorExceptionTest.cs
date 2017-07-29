using FluentAssertions;
using NPoint.Exceptions;
using System;
using System.Net.Http;
using Xunit;

namespace NPoint.Tests.Exceptions
{
    public class ServerErrorExceptionTest
    {
        [Theory]
        [InlineData(true, false, "response")]
        [InlineData(false, true, "request")]
        public void ShouldThrowExceptionWhenNullRequestIsPassedToCtor(bool requestSpecified, bool responseSpecified, string paramName)
        {
            // Arrange
            var request = requestSpecified ? new HttpRequestMessage() : default(HttpRequestMessage);
            var response = responseSpecified ? new HttpResponseMessage() : default(HttpResponseMessage);

            // Act
            Action activity = () => new ServerErrorException(request, response);

            // Assert
            activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(paramName);
        }

        [Theory, NPointData]
        public void ShouldThrowServerErrorException(HttpRequestMessage request, HttpResponseMessage response)
        {
            // Arrange
            // Act
            Action activity = () => { throw new ServerErrorException(request, response); };

            // Assert
            var assertion = activity.ShouldThrowExactly<ServerErrorException>().WithMessage("Server responded with a non-OK status");
            assertion.And.Request.ShouldBeEquivalentTo(request);
            assertion.And.Response.ShouldBeEquivalentTo(response);
        }
    }
}
