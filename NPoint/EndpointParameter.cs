using NPoint.Transport;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NPoint
{
    public class EndpointParameter
    {
        public static readonly int DefaultTimeout = 60;

        public Action<HttpRequestMessage> OnRequestReady { get; set; }

        public Action<HttpResponseMessage> OnResponseReceived { get; set; }

        public Action<object> OnResponseConverted { get; set; }

        public IEnumerable<Action<IHttpRequestBuilder>> RequestSpecs { get; set; }

        public int Timeout { get; set; }

        public EndpointParameter()
        {
            Timeout = DefaultTimeout;
        }
    }
}