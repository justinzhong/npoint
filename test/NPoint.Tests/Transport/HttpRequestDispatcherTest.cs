using FluentAssertions;
using NPoint.Exceptions;
using NPoint.Filters;
using NPoint.Transport;
using NSubstitute;
using Ploeh.AutoFixture;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NPoint.Tests.Transport
{
    public class HttpRequestDispatcherTest
    {
        [Theory]
        [InlineData(true, false, "httpClientFactory")]
        [InlineData(false, true, "filterRegistry")]
        public void ShouldThrowArgumentNullExceptionsWhenNullArgsPassedToCtor(bool filterRegistrySpecified, bool httpClientFactorySpecified, string argName)
        {
            // Arrange
            var filterRegistry = filterRegistrySpecified ? Substitute.For<IRequestFilterRegistry>() : default(IRequestFilterRegistry);
            var httpClientFactory = httpClientFactorySpecified ? Substitute.For<IHttpClientFactory>() : default(IHttpClientFactory);

            // Act
            Action activity = () => new HttpRequestDispatcher(filterRegistry, httpClientFactory);

            // Assert
            activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(argName);
        }

        [Theory, AutoNSubstituteData]
        public void ShouldThrowArgumentNullExceptionWhenRequestIsNull(IHttpClientFactory httpClientFactory, IRequestFilterRegistry filterRegistry)
        {
            // Arrange
            var request = default(HttpRequestMessage);
            var timeout = new Fixture().Create<int>();

            // Act
            var sut = new HttpRequestDispatcher(filterRegistry, httpClientFactory);
            Func<Task<string>> activity = async () => await sut.DispatchRequest(request, timeout);

            // Assert
            activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(nameof(request));
        }

        [Theory, AutoNSubstituteData]
        public void ShouldThrowTimeoutException(IHttpClientFactory httpClientFactory, IRequestFilterRegistry filterRegistry)
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
            filterRegistry.Filters.Returns(new List<Func<IRequestFilter>>());

            // Act
            var sut = new HttpRequestDispatcher(filterRegistry, httpClientFactory);
            Func<Task<string>> activity = async () => await sut.DispatchRequest(request, timeout);

            // Assert
            var assertion = activity.ShouldThrowExactly<NetworkTimeoutException>().WithMessage($"Did not receive any response from the server after {timeout} seconds");
            assertion.And.Request.ShouldBeEquivalentTo(request);
            assertion.And.Timeout.ShouldBeEquivalentTo(timeout);
        }

        [Theory, AutoNSubstituteData]
        public void ShouldThrowServerErrorException(IHttpClientFactory httpClientFactory, IRequestFilterRegistry filterRegistry)
        {
            // Arrange
            var fixture = new Fixture().Customize(new NPointCustomizations());
            var request = fixture.Create<HttpRequestMessage>();
            var timeout = fixture.Create<int>();
            var mockHttp = new MockHttpMessageHandler();
            var expectedStatusCode = HttpStatusCode.NotFound;
            mockHttp.When(request.Method, request.RequestUri.ToString())
                .Respond(expectedStatusCode);
            httpClientFactory.Create().Returns(mockHttp.ToHttpClient());
            filterRegistry.Filters.Returns(new List<Func<IRequestFilter>>());

            // Act
            var sut = new HttpRequestDispatcher(filterRegistry, httpClientFactory);
            Func<Task<string>> activity = async () => await sut.DispatchRequest(request, timeout);

            // Assert
            var assertion = activity.ShouldThrowExactly<ServerErrorException>();
            assertion.And.Request.ShouldBeEquivalentTo(request);
            assertion.And.Response.StatusCode.ShouldBeEquivalentTo(expectedStatusCode);
        }

        [Theory, AutoNSubstituteData]
        public async Task ShouldReturnServerResponse(IHttpClientFactory httpClientFactory, IRequestFilterRegistry filterRegistry)
        {
            // Arrange
            var fixture = new Fixture().Customize(new NPointCustomizations());
            var request = fixture.Create<HttpRequestMessage>();
            var timeout = fixture.Create<int>();
            var mockHttp = new MockHttpMessageHandler();
            var expectedContent = "{ }";
            mockHttp.When(request.Method, request.RequestUri.ToString())
                .Respond(new StringContent(expectedContent));
            httpClientFactory.Create().Returns(mockHttp.ToHttpClient());
            filterRegistry.Filters.Returns(new List<Func<IRequestFilter>>());

            // Act
            var sut = new HttpRequestDispatcher(filterRegistry, httpClientFactory);
            var actualContent = await sut.DispatchRequest(request, timeout);

            // Assert
            actualContent.ShouldBeEquivalentTo(expectedContent);
        }

        [Theory, AutoNSubstituteData]
        public async Task ShouldFilterRequest(IHttpClientFactory httpClientFactory, IRequestFilterRegistry filterRegistry)
        {
            // Arrange
            var fixture = new Fixture().Customize(new NPointCustomizations());
            var request = fixture.Create<HttpRequestMessage>();
            var timeout = fixture.Create<int>();
            var customHeader = "X-Custom-Header";
            var customValue = "Custom-Value";
            var mockFilter = Substitute.For<IRequestFilter>();
            var mockHttp = new MockHttpMessageHandler();
            var expectedContent = "{ }";
            request.Method = HttpMethod.Put;
            mockHttp.Expect(request.Method, request.RequestUri.ToString())
                .WithHeaders(customHeader, customValue)
                .Respond(new StringContent(expectedContent));
            httpClientFactory.Create().Returns(mockHttp.ToHttpClient());
            mockFilter.When(f => f.Filter(request)).Do(c => c.Arg<HttpRequestMessage>().Headers.Add(customHeader, customValue));
            filterRegistry.Filters.Returns(new List<Func<IRequestFilter>> { () => new CustomRequestFilter(customHeader, customValue) });

            // Act
            var sut = new HttpRequestDispatcher(filterRegistry, httpClientFactory);
            var actualContent = await sut.DispatchRequest(request, timeout);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            actualContent.ShouldBeEquivalentTo(expectedContent);
        }

        class CustomRequestFilter : IRequestFilter
        {
            private string CustomHeader { get; }
            private string CustomValue { get; }

            public CustomRequestFilter(string customHeader, string customValue)
            {
                CustomHeader = customHeader;
                CustomValue = customValue;
            }

            public void Filter(HttpRequestMessage request)
            {
                request.Headers.Add(CustomHeader, CustomValue);
            }
        }
    }
}
