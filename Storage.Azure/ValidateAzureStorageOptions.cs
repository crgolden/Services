namespace Services
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    /// <inheritdoc />
    [PublicAPI]
    public class ValidateAzureStorageOptions : IValidateOptions<AzureStorageOptions>
    {
        /// <inheritdoc />
        public ValidateOptionsResult Validate(string name, AzureStorageOptions options)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return IsNullOrWhiteSpace(options.AccountName) ||
                   (IsNullOrWhiteSpace(options.AccountKey1) && IsNullOrWhiteSpace(options.AccountKey2))
                ? Fail($"'{nameof(AzureStorageOptions)}' section is invalid")
                : Success;
        }
    }
}
