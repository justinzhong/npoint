using System;
using System.Net.Http;

namespace NPoint.Exceptions
{
    public class NetworkTimeoutException : Exception
    {
        private readonly static string TimeoutErrorMessage = "Did not receive any response from the server after {0} seconds";

        public override string Message { get; }

        public HttpRequestMessage Request { get; }

        public int Timeout { get; }

        public NetworkTimeoutException(HttpRequestMessage request, int timeout) : base()
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            Message = $"{string.Format(TimeoutErrorMessage, timeout)}";
            Timeout = timeout;
            Request = request;
        }
    }
}