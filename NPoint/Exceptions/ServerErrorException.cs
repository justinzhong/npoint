using System;
using System.Net.Http;

namespace NPoint.Exceptions
{
    public class ServerErrorException : Exception
    {
        public override string Message => "Server responded with a non-OK status";

        public HttpRequestMessage Request { get; }

        public HttpResponseMessage Response { get; }

        public ServerErrorException(HttpRequestMessage request, HttpResponseMessage response) : base()
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (response == null) throw new ArgumentNullException(nameof(response));

            Request = request;
            Response = response;
        }
    }
}
