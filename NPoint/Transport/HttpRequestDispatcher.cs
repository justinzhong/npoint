using NPoint.Exceptions;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NPoint.Transport
{
    public class HttpRequestDispatcher : IHttpRequestDispatcher
    {
        private IHttpClientFactory HttpClientFactory { get; }

        public HttpRequestDispatcher() : this(new HttpClientFactory()) { }

        public HttpRequestDispatcher(IHttpClientFactory httpClientFactory)
        {
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));

            HttpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> Dispatch(HttpRequestMessage request, int timeout)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (timeout < 0) throw new ArgumentOutOfRangeException(nameof(timeout), $"Timeout must be a non-negative value but received {timeout}");

            var cancellationSource = new CancellationTokenSource();

            try
            {
                using (var client = HttpClientFactory.Create())
                {
                    client.Timeout = TimeSpan.FromSeconds(timeout);
                    var response = await client.SendAsync(request, cancellationSource.Token);

                    return response;
                }
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cancellationSource.Token) throw;

                throw new NetworkTimeoutException(request, timeout);
            }
        }
    }
}
