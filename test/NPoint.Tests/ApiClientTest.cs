using FluentAssertions;
using NPoint.Config;
using NPoint.Filters;
using NPoint.Serialization;
using NPoint.Tests.Data;
using NSubstitute;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace NPoint.Tests
{
    public class ApiClientTest
    {
        public class WhenConstructing
        {
            [Theory]
            [InlineData(false, true, "endpointFactory")]
            [InlineData(true, false, "config")]
            public void ShouldNotAcceptNullDependencies(bool isConfigNull, bool isEndpointFactoryNull, string paramName)
            {
                // Arrange
                var config = isConfigNull ? default(IApiConfig) : new DefaultApiConfig();
                var endpointFactory = isEndpointFactoryNull ? default(IEndpointFactory) : new EndpointFactory();

                // Act
                Action activity = () => new ApiClient(config, endpointFactory);

                // Assert
                activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(paramName);
            }

            [Theory, NPointData(true)]
            public void ShouldAcceptConfigOnly(IApiConfig config)
            {
                // Arrange

                // Act
                Action activity = () => new ApiClient(config);

                // Assert
                activity.ShouldNotThrow();
            }
        }

        public class WhenCallingApi
        {
            [Fact]
            public void ShouldNotAcceptNullDependency()
            {
                // Arrange
                var endpointSpec = default(Action<IEndpoint>);

                // Act
                var sut = new ApiClient();
                Func<Task<CustomResponseModel>> activity = async () => await sut.Call<CustomResponseModel>(endpointSpec);

                // Assert
                activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(nameof(endpointSpec));
            }

            [Theory, NPointData(true)]
            public async Task ShouldApplyHttpRequestFilters(
                IHttpRequestFilterConfig config, 
                IEndpointFactory endpointFactory, 
                IEndpoint endpoint,
                IHttpRequestFilter httpRequestFilter,
                HttpRequestMessage httpRequest,
                HttpResponseMessage httpResponse,
                CustomResponseModel modelResponse)
            {
                // Arrange
                var customAcceptHeader = "application/vnd.npoint.apiclient+json";
                httpRequestFilter.Filter(Arg.Do<HttpRequestMessage>(message => message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(customAcceptHeader))));
                config.HttpRequestFilter.Returns(httpRequestFilter);
                var actualFilter = default(Action<HttpRequestMessage>);
                endpoint.Call(Arg.Any<Func<HttpResponseMessage, Task<CustomResponseModel>>>()).Returns(Task.FromResult(modelResponse));
                endpointFactory.Create(Arg.Do<EndpointParameter>(parameter => actualFilter = parameter.OnRequestReady)).Returns(endpoint);

                // Act
                var sut = new ApiClient(config, endpointFactory);
                var actualModelResponse = await sut.Call<CustomResponseModel>(_ => { });

                // Assert
                actualFilter.Should().NotBeNull();
                actualFilter(httpRequest);
                httpRequest.Headers.Accept.Contains(new MediaTypeWithQualityHeaderValue(customAcceptHeader));
            }

            [Theory, NPointData(true)]
            public async Task ShouldApplyHttpResponseFilters(
                IHttpResponseFilterConfig config,
                IEndpointFactory endpointFactory,
                IEndpoint endpoint,
                IHttpResponseFilter httpResponseFilter,
                HttpRequestMessage httpRequest,
                HttpResponseMessage expectedResponseMessage,
                CustomResponseModel modelResponse)
            {
                // Arrange
                var actualResponseMessage = default(HttpResponseMessage);
                httpResponseFilter.Filter(Arg.Do<HttpResponseMessage>(message => actualResponseMessage = message));
                config.HttpResponseFilter.Returns(httpResponseFilter);
                var actualFilter = default(Action<HttpResponseMessage>);
                endpoint.Call(Arg.Any<Func<HttpResponseMessage, Task<CustomResponseModel>>>()).Returns(Task.FromResult(modelResponse));
                endpointFactory.Create(Arg.Do<EndpointParameter>(parameter => actualFilter = parameter.OnResponseReceived)).Returns(endpoint);

                // Act
                var sut = new ApiClient(config, endpointFactory);
                var actualModelResponse = await sut.Call<CustomResponseModel>(_ => { });

                // Assert
                actualFilter.Should().NotBeNull();
                actualFilter(expectedResponseMessage);
                actualResponseMessage.ShouldBeEquivalentTo(expectedResponseMessage);
            }

            [Theory, NPointData(true)]
            public async Task ShouldApplyModelResponseFilters(
                IModelFilterConfig config,
                IEndpointFactory endpointFactory,
                IEndpoint endpoint,
                IModelFilter modelFilter,
                HttpRequestMessage httpRequest,
                HttpResponseMessage expectedResponseMessage,
                CustomResponseModel expectedModel)
            {
                // Arrange
                var actualModel = default(CustomResponseModel);
                modelFilter.Filter(Arg.Do<CustomResponseModel>(model => actualModel = model));
                config.ModelFilter.Returns(modelFilter);
                var actualFilter = default(Action<CustomResponseModel>);
                endpoint.Call(Arg.Any<Func<HttpResponseMessage, Task<CustomResponseModel>>>()).Returns(Task.FromResult(expectedModel));
                endpointFactory.Create(Arg.Do<EndpointParameter>(parameter => actualFilter = parameter.OnResponseConverted)).Returns(endpoint);

                // Act
                var sut = new ApiClient(config, endpointFactory);
                var actualModelResponse = await sut.Call<CustomResponseModel>(_ => { });

                // Assert
                actualFilter.Should().NotBeNull();
                actualFilter(expectedModel);
                actualModel.ShouldBeEquivalentTo(expectedModel);
            }

            [Theory, NPointData(true)]
            public async Task ShouldConvertResponseToModel(
                IResponseConverter responseConverter,
                IApiConfig config,
                IEndpoint endpoint,
                IEndpointFactory endpointFactory,
                HttpRequestMessage httpRequest,
                HttpResponseMessage httpResponse,
                CustomResponseModel expectedModel)
            {
                // Arrange
                responseConverter.Convert<CustomResponseModel>(httpResponse).Returns(Task.FromResult(expectedModel));
                config.ResponseConverter.Returns(responseConverter);
                var actualConverterDelegate = default(Func<HttpResponseMessage, Task<CustomResponseModel>>);
                endpoint.Call(Arg.Do<Func<HttpResponseMessage, Task<CustomResponseModel>>>(converterDelgate => actualConverterDelegate = converterDelgate)).Returns(Task.FromResult(expectedModel));
                endpointFactory.Create(Arg.Any<EndpointParameter>()).Returns(endpoint);

                // Act
                var sut = new ApiClient(config, endpointFactory);
                await sut.Call<CustomResponseModel>(_ => { });

                // Assert
                actualConverterDelegate.Should().NotBeNull();
                var actualModel = await actualConverterDelegate(httpResponse);
                actualModel.ShouldBeEquivalentTo(expectedModel);
            }

            [Theory, NPointData(true)]
            public async Task ShouldApplySpecificationOnEndpoint(
                IApiConfig config,
                IEndpoint endpoint,
                IEndpointFactory endpointFactory,
                HttpRequestMessage httpRequest,
                HttpResponseMessage httpResponse,
                CustomResponseModel expectedModel,
                Uri expectedUrl)
            {
                // Arrange
                var expectedBody = @"{""id"":100}";
                var expectedContentType = "application/json";
                endpoint.Put(expectedUrl, expectedBody, expectedContentType).Returns(endpoint);
                endpointFactory.Create(Arg.Any<EndpointParameter>()).Returns(endpoint);
                Action<IEndpoint> endpointSpec = e => e.Put(expectedUrl, expectedBody, expectedContentType);

                // Act
                var sut = new ApiClient(config, endpointFactory);
                await sut.Call<CustomResponseModel>(endpointSpec);

                // Assert
                endpoint.Received(1).Put(Arg.Is(expectedUrl), Arg.Is(expectedBody), Arg.Is(expectedContentType));
            }
        }
    }
}
