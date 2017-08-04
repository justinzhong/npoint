using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NPoint.Serialization
{
    public class JsonResponseConverter : IResponseConverter
    {
        private static readonly string SupportedContentType = "application/json";

        private IJsonSerializer Serializer { get; }

        public JsonResponseConverter() : this(new JsonNetJsonSerializer()) { }

        public JsonResponseConverter(IJsonSerializer serializer)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            Serializer = serializer;
        }

        public async Task<TResponse> Convert<TResponse>(HttpResponseMessage response) where TResponse : class
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            ValidateContent(response);

            var body = await response.Content.ReadAsStringAsync();

            return Serializer.Deserialize<TResponse>(body);
        }

        private void ValidateContent(HttpResponseMessage response)
        {
            if (response.Content == null) throw new ArgumentException("Response has no content body", nameof(response));

            var responseContentType = response.Content.Headers.ContentType.MediaType;

            if (string.Equals(responseContentType, SupportedContentType, StringComparison.OrdinalIgnoreCase)) return;

            throw new NotSupportedException($"The supported content type is {SupportedContentType} but the response content type is {responseContentType}");
        }
    }
}