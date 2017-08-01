using System;
using System.Collections.Generic;
using NPoint.Transport;
using System.Net.Http;

namespace NPoint
{
    public class EndpointParameter
    {
        public Action<HttpResponseMessage> Callback { get; set; }

        public IEnumerable<Action<IHttpRequestBuilder>> RequestSpecs { get; set; }

        public HttpRequestMessage Request { get; set; }

        public int Timeout { get; set; }
    }
}