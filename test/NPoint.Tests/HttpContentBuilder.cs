using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using System.Net.Http;
using System.Net.Http.Headers;
using System;

namespace NPoint.Tests
{

    public class HttpContentBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var type = request as Type;

            if (type == null) return new NoSpecimen();

            if (type != typeof(HttpContent)) return new NoSpecimen();

            var stringContent = new StringContent(context.Create<string>());
            stringContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

            return stringContent;
        }
    }
}