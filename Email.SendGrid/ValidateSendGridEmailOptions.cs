namespace Services
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    /// <inheritdoc />
    [PublicAPI]
    public class ValidateSendGridEmailOptions : IValidateOptions<SendGridEmailOptions>
    {
        /// <inheritdoc />
        public ValidateOptionsResult Validate(string name, SendGridEmailOptions options)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return IsNullOrWhiteSpace(options.ApiKey)
                ? Fail($"'{nameof(SendGridEmailOptions)}' section is invalid")
                : Success;
        }
    }
}
