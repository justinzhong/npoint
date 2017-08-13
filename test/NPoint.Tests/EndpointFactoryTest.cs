using FluentAssertions;
using NPoint.Transport;
using NSubstitute;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace NPoint.Tests
{
    public class EndpointFactoryTest
    {
        [Theory]
        [InlineData(false, true, "requestDispatcher")]
        [InlineData(true, false, "requestBuilderFactory")]
        public void ShouldNotAcceptNullArgs(bool requestBuilderFactoryIsNull, bool requestDispatcherIsNull, string paramName)
        {
            // Arrange
            var requestBuilderFactory = requestBuilderFactoryIsNull ? default(IHttpRequestBuilderFactory) : new HttpRequestBuilderFactory();
            var requestDispatcher = requestDispatcherIsNull ? default(IHttpRequestDispatcher) : new HttpRequestDispatcher();

            // Act
            Action activity = () => new EndpointFactory(requestBuilderFactory, requestDispatcher);

            // Assert
            activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(paramName);
        }

        [Theory, NPointData(true)]
        public async Task ShouldReferenceDependencies(
            IHttpRequestBuilder requestBuilder,
            IHttpRequestBuilderFactory requestBuilderFactory,
            IHttpRequestDispatcher requestDispatcher,
            HttpRequestMessage request,
            HttpResponseMessage response,
            EndpointParameter parameter)
        {
            // Arrange
            requestBuilder.Build().Returns(request);
            requestBuilderFactory.Create().Returns(requestBuilder);
            requestDispatcher.Dispatch(request, parameter.Timeout).Returns(Task.FromResult(response));

            // Act
            var sut = new EndpointFactory(requestBuilderFactory, requestDispatcher);
            var endpoint = sut.Create(parameter);
            var actualResponse = endpoint.CallThrough();

            // Assert
            requestBuilder.Received(1).Build();
            requestBuilderFactory.Received(1).Create();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            requestDispatcher.Received(1).Dispatch(Arg.Is(request), Arg.Is(parameter.Timeout));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
