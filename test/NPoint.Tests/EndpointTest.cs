namespace NPoint.Tests
{
    public partial class EndpointTest
    {
        //[Theory, AutoNSubstituteData]
        //public async Task ShouldDispatchRequestOfStringType(INPointConfig config,
        //    IHttpRequestBuilderFactory requestBuilderFactory,
        //    IHttpRequestBuilder requestBuilder,
        //    IHttpRequestDispatcher requestDispatcher,
        //    IResponseParser responseParser)
        //{
        //    // Arrange
        //    var fixture = new Fixture();
        //    fixture.Customize(new NPointCustomizations());
        //    var request = fixture.Create<HttpRequestMessage>();
        //    config.RequestTimeout.Returns(fixture.Create<int>());
        //    var response = "{ }";
        //    var receivedRequest = default(HttpRequestMessage);
        //    var receivedTimeout = default(int);
        //    requestDispatcher.DispatchRequest(Arg.Do<HttpRequestMessage>(r => receivedRequest = r), Arg.Do<int>(t => receivedTimeout = t)).Returns(Task.FromResult(response));
        //    requestBuilderFactory.Create().Returns(requestBuilder);

        //    // Act
        //    var sut = new Endpoint(config, requestBuilderFactory, requestDispatcher, responseParser);
        //    var actualResponse = await sut.Invoke(request);

        //    // Assert
        //    var timeout = config.Received().RequestTimeout;
        //    actualResponse.ShouldBeEquivalentTo(response);
        //    receivedRequest.ShouldBeEquivalentTo(request);
        //    receivedTimeout.ShouldBeEquivalentTo(config.RequestTimeout);
        //}

        //[Theory, AutoNSubstituteData]
        //public async Task ShouldDispatchRequestSpecOfStringType(INPointConfig config,
        //    IHttpRequestBuilderFactory requestBuilderFactory,
        //    IHttpRequestBuilder requestBuilder,
        //    IHttpRequestDispatcher requestDispatcher,
        //    IResponseParser responseParser)
        //{
        //    // Arrange
        //    var fixture = new Fixture();
        //    fixture.Customize(new NPointCustomizations());
        //    config.RequestTimeout.Returns(fixture.Create<int>());
        //    var response = "{ }";
        //    var receivedRequest = default(HttpRequestMessage);
        //    var receivedTimeout = default(int);
        //    var requestUrl = fixture.Create<Uri>();
        //    requestBuilderFactory.Create().Returns(requestBuilder);
        //    requestDispatcher.DispatchRequest(Arg.Do<HttpRequestMessage>(r => receivedRequest = r), Arg.Do<int>(t => receivedTimeout = t)).Returns(Task.FromResult(response));
        //    Action<IHttpRequestBuilder> requestSpec = builder =>
        //    {
        //        builder.SetEndpoint(requestUrl);
        //    };

        //    // Act
        //    var sut = new Endpoint(config, requestBuilderFactory, requestDispatcher, responseParser);
        //    var actualResponse = await sut.Invoke(requestSpec);

        //    // Assert
        //    var timeout = config.Received().RequestTimeout;
        //    actualResponse.ShouldBeEquivalentTo(response);
        //    receivedRequest.ShouldBeEquivalentTo(requestBuilder.Request);
        //    receivedTimeout.ShouldBeEquivalentTo(config.RequestTimeout);
        //    requestBuilder.Received(1).SetEndpoint(Arg.Is(requestUrl));
        //}

        //[Theory, AutoNSubstituteData]
        //public async Task ShouldDispatchRequestOfCustomType(INPointConfig config,
        //    IHttpRequestBuilderFactory requestBuilderFactory,
        //    IHttpRequestBuilder requestBuilder,
        //    IHttpRequestDispatcher requestDispatcher,
        //    IResponseParser responseParser)
        //{
        //    // Arrange
        //    var fixture = new Fixture();
        //    fixture.Customize(new NPointCustomizations());
        //    var request = fixture.Create<HttpRequestMessage>();
        //    config.RequestTimeout.Returns(fixture.Create<int>());
        //    var response = "{ }";
        //    var receivedRequest = default(HttpRequestMessage);
        //    var receivedTimeout = default(int);
        //    var expectedResponse = fixture.Create<CustomResponseModel>();
        //    requestBuilderFactory.Create().Returns(requestBuilder);
        //    requestDispatcher.DispatchRequest(Arg.Do<HttpRequestMessage>(r => receivedRequest = r), Arg.Do<int>(t => receivedTimeout = t)).Returns(Task.FromResult(response));
        //    responseParser.ParseResponse<CustomResponseModel>(response).Returns(expectedResponse);

        //    // Act
        //    var sut = new Endpoint(config, requestBuilderFactory, requestDispatcher, responseParser);
        //    var actualResponse = await sut.Invoke<CustomResponseModel>(request);

        //    // Assert
        //    var timeout = config.Received().RequestTimeout;
        //    actualResponse.ShouldBeEquivalentTo(expectedResponse);
        //    receivedRequest.ShouldBeEquivalentTo(request);
        //    receivedTimeout.ShouldBeEquivalentTo(config.RequestTimeout);
        //    responseParser.Received(1).ParseResponse<CustomResponseModel>(Arg.Is(response));
        //}

        //[Theory, AutoNSubstituteData]
        //public async Task ShouldDispatchRequestSpecOfCustomType(INPointConfig config,
        //    IHttpRequestBuilderFactory requestBuilderFactory,
        //    IHttpRequestBuilder requestBuilder,
        //    IHttpRequestDispatcher requestDispatcher,
        //    IResponseParser responseParser)
        //{
        //    // Arrange
        //    var fixture = new Fixture();
        //    fixture.Customize(new NPointCustomizations());
        //    config.RequestTimeout.Returns(fixture.Create<int>());
        //    var response = "{ }";
        //    var receivedRequest = default(HttpRequestMessage);
        //    var receivedTimeout = default(int);
        //    var requestUrl = fixture.Create<Uri>();
        //    var expectedResponse = fixture.Create<CustomResponseModel>();
        //    requestBuilderFactory.Create().Returns(requestBuilder);
        //    requestDispatcher.DispatchRequest(Arg.Do<HttpRequestMessage>(r => receivedRequest = r), Arg.Do<int>(t => receivedTimeout = t)).Returns(Task.FromResult(response));
        //    responseParser.ParseResponse<CustomResponseModel>(response).Returns(expectedResponse);
        //    Action<IHttpRequestBuilder> requestSpec = builder =>
        //    {
        //        builder.SetEndpoint(requestUrl);
        //    };

        //    // Act
        //    var sut = new Endpoint(config, requestBuilderFactory, requestDispatcher, responseParser);
        //    var actualResponse = await sut.Invoke<CustomResponseModel>(requestSpec);

        //    // Assert
        //    var timeout = config.Received().RequestTimeout;
        //    actualResponse.ShouldBeEquivalentTo(expectedResponse);
        //    receivedRequest.ShouldBeEquivalentTo(requestBuilder.Request);
        //    receivedTimeout.ShouldBeEquivalentTo(config.RequestTimeout);
        //    responseParser.Received(1).ParseResponse<CustomResponseModel>(Arg.Is(response));
        //    requestBuilder.Received(1).SetEndpoint(Arg.Is(requestUrl));
        //}
    }
}
