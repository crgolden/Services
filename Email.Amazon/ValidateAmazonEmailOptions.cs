namespace Services
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    /// <inheritdoc />
    [PublicAPI]
    public class ValidateAmazonEmailOptions : IValidateOptions<AmazonEmailOptions>
    {
        /// <inheritdoc />
        public ValidateOptionsResult Validate(string name, AmazonEmailOptions options)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return IsNullOrWhiteSpace(options.AccessKeyId) ||
                   IsNullOrWhiteSpace(options.SecretAccessKey)
                ? Fail($"'{nameof(AmazonEmailOptions)}' section is invalid")
                : Success;
        }
    }
}
