using NPoint.Transport;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NPoint
{
    public class EndpointParameter
    {
        public Action<HttpResponseMessage> Callback { get; set; }

        public IEnumerable<Action<IHttpRequestBuilder>> RequestSpecs { get; set; }

        public int Timeout { get; set; }
    }
}