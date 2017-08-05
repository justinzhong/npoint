using FluentAssertions;
using NPoint.Exceptions;
using NPoint.Transport;
using NSubstitute;
using Ploeh.AutoFixture;
using RichardSzalay.MockHttp;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NPoint.Tests.Transport
{
    public class HttpRequestDispatcherTest
    {
        [Fact]
        public void ShouldThrowArgumentNullExceptionsFromCtor()
        {
            // Arrange
            var httpClientFactory = default(IHttpClientFactory);

            // Act
            Action activity = () => new HttpRequestDispatcher(httpClientFactory);

            // Assert
            activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(nameof(httpClientFactory));
        }

        [Theory, NPointData(true)]
        public void ShouldThrowArgumentNullExceptionWhenRequestIsNull(IHttpClientFactory httpClientFactory)
        {
            // Arrange
            var request = default(HttpRequestMessage);
            var timeout = new Fixture().Create<int>();

            // Act
            var sut = new HttpRequestDispatcher(httpClientFactory);
            Func<Task<HttpResponseMessage>> activity = async () => await sut.Dispatch(request, timeout);

            // Assert
            activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(nameof(request));
        }

        [Theory, NPointData(true)]
        public void ShouldThrowTimeoutException(IHttpClientFactory httpClientFactory)
        {
            // Arrange
            var request = new Fixture().Customize(new NPointCustomizations()).Create<HttpRequestMessage>();
            var timeout = 1; // Immediate timeout after 1 second
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(request.Method, request.RequestUri.ToString())
                .Respond(async () => {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                return await Task.FromResult(default(HttpResponseMessage));
            });
            httpClientFactory.Create().Returns(mockHttp.ToHttpClient());

            // Act
            var sut = new HttpRequestDispatcher(httpClientFactory);
            Func<Task<HttpResponseMessage>> activity = async () => await sut.Dispatch(request, timeout);

            // Assert
            var assertion = activity.ShouldThrowExactly<NetworkTimeoutException>();
            assertion.WithMessage($"Did not receive any response from the server after {timeout} seconds");
            assertion.And.Request.ShouldBeEquivalentTo(request);
            assertion.And.Timeout.ShouldBeEquivalentTo(timeout);
        }

        [Theory, NPointData(true)]
        public void ShouldThrowServerErrorException(IHttpClientFactory httpClientFactory)
        {
            // Arrange
            var fixture = new Fixture().Customize(new NPointCustomizations());
            var request = fixture.Create<HttpRequestMessage>();
            var timeout = fixture.Create<int>();
            var mockHttp = new MockHttpMessageHandler();
            var expected = new WebException("Network connection lost", WebExceptionStatus.ConnectionClosed);

            mockHttp.When(request.Method, request.RequestUri.ToString())
                .Respond(() => { throw expected; });
            httpClientFactory.Create().Returns(mockHttp.ToHttpClient());

            // Act
            var sut = new HttpRequestDispatcher(httpClientFactory);
            Func<Task<HttpResponseMessage>> activity = async () => await sut.Dispatch(request, timeout);

            // Assert
            var assertion = activity.ShouldThrowExactly<AggregateException>();
            assertion.And.InnerException.ShouldBeEquivalentTo(expected);
        }

        [Theory, NPointData(true)]
        public async Task ShouldReturnServerResponse(IHttpClientFactory httpClientFactory, HttpResponseMessage expected)
        {
            // Arrange
            var fixture = new Fixture().Customize(new NPointCustomizations());
            var request = fixture.Create<HttpRequestMessage>();
            var timeout = fixture.Create<int>();
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(request.Method, request.RequestUri.ToString())
                .Respond(expected);
            httpClientFactory.Create().Returns(mockHttp.ToHttpClient());

            // Act
            var sut = new HttpRequestDispatcher(httpClientFactory);
            var actual = await sut.Dispatch(request, timeout);

            // Assert
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
