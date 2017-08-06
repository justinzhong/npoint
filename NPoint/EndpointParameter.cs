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

        public List<Action<IHttpRequestBuilder>> RequestSpecs { get; }

        public int Timeout { get; set; }

        public EndpointParameter()
        {
            RequestSpecs = new List<Action<IHttpRequestBuilder>>();
            Timeout = DefaultTimeout;
        }

        public EndpointParameter(EndpointParameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            OnRequestReady = parameter.OnRequestReady;
            OnResponseReceived = parameter.OnResponseReceived;
            OnResponseConverted = parameter.OnResponseConverted;
            RequestSpecs = parameter.RequestSpecs;
            Timeout = parameter.Timeout;
        }
    }
}