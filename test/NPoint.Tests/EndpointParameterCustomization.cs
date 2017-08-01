using NPoint.Transport;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;

namespace NPoint.Tests
{
    public class EndpointParameterCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<EndpointParameter>(composer =>
                composer
                    .With(parameter => parameter.RequestSpecs, new List<Action<IHttpRequestBuilder>>())
                    .With(parameter => parameter.Timeout, fixture.Create<int>()));
        }
    }
}