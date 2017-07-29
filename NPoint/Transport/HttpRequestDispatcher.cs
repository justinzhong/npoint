using NPoint.Exceptions;
using NPoint.Filters;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NPoint.Transport
{
    public class HttpRequestDispatcher : IHttpRequestDispatcher
    {
        private IRequestFilterRegistry FilterRegistry { get; }
        private IHttpClientFactory HttpClientFactory { get; }

        public HttpRequestDispatcher(IHttpClientFactory httpClientFactory) : this(new DefaultFilterRegistry(), httpClientFactory) { }

        public HttpRequestDispatcher(IRequestFilterRegistry filterRegistry, IHttpClientFactory httpClientFactory)
        {
            if (filterRegistry == null) throw new ArgumentNullException(nameof(filterRegistry));
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));

            FilterRegistry = filterRegistry;
            HttpClientFactory = httpClientFactory;
        }

        public async Task<string> DispatchRequest(HttpRequestMessage request, int timeout)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (timeout <= 0) throw new ArgumentOutOfRangeException("Timeout must be a positive value", nameof(timeout));

            ApplyFilers(request);

            var response = string.Empty;
            var cancellationSource = new CancellationTokenSource();

            try
            {
                using (var client = HttpClientFactory.Create())
                {
                    client.Timeout = TimeSpan.FromSeconds(timeout);

                    var responseString = await ReadResponse(client, request, cancellationSource.Token);

                    return responseString;
                }
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cancellationSource.Token)
                {
                    throw;
                }

                throw new NetworkTimeoutException(request, timeout);
            }
        }

        private void ApplyFilers(HttpRequestMessage request)
        {
            FilterRegistry.Filters.ForEach(filter => filter().Filter(request));
        }

        private async Task<string> ReadResponse(HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

            // TODO: replace this section with IResponseValidator extension point
            // where the default behaviour is any non-OK status yields a ServerErrorException
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ServerErrorException(request, response);
            }

            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}