using NPoint.Config;
using NPoint.Serialization;
using NPoint.Validators;
using System;

namespace NPoint.Parsers
{
    public class ResponseParser : IResponseParser
    {
        private INPointConfig Config { get; }
        private IJsonSerializer JsonSerializer { get; }
        private IValidatorRegistry ValidatorRegistry { get; }

        public ResponseParser(INPointConfig config, IJsonSerializer jsonSerializer, IValidatorRegistry validatorRegistry)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (jsonSerializer == null) throw new ArgumentNullException(nameof(jsonSerializer));
            if (validatorRegistry == null) throw new ArgumentNullException(nameof(validatorRegistry));

            Config = config;
            JsonSerializer = jsonSerializer;
            ValidatorRegistry = validatorRegistry;
        }

        TResponse IResponseParser.ParseResponse<TResponse>(string response)
        {
            if (string.IsNullOrEmpty(response)) throw new ArgumentException("String cannot be empty or null", nameof(response));

            var subject = JsonSerializer.Deserialize<TResponse>(response);

            return ValidateResponse(subject);
        }

        private TResponse ValidateResponse<TResponse>(TResponse subject)
            where TResponse : class
        {
            if (!CanBeValidated<TResponse>()) return subject;

            var validator = ValidatorRegistry.GetValidator<TResponse>();

            validator.Validate(subject);

            return subject;
        }

        private bool CanBeValidated<TResponse>()
            where TResponse : class
        {
            return ValidatorRegistry.HasValidatorFor<TResponse>();
        }
    }
}