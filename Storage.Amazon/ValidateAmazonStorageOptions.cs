namespace Services
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    /// <inheritdoc />
    [PublicAPI]
    public class ValidateAmazonStorageOptions : IValidateOptions<AmazonStorageOptions>
    {
        /// <inheritdoc />
        public ValidateOptionsResult Validate(string name, AmazonStorageOptions options)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return IsNullOrWhiteSpace(options.AccessKeyId) || IsNullOrWhiteSpace(options.SecretAccessKey)
                ? Fail($"'{nameof(AmazonStorageOptions)}' section is invalid")
                : Success;
        }
    }
}
