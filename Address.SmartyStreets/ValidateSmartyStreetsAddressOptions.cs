namespace Services
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    /// <inheritdoc />
    [PublicAPI]
    public class ValidateSmartyStreetsAddressOptions : IValidateOptions<SmartyStreetsAddressOptions>
    {
        /// <inheritdoc />
        public ValidateOptionsResult Validate(string name, SmartyStreetsAddressOptions options)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return IsNullOrWhiteSpace(options.AuthId) ||
                   IsNullOrWhiteSpace(options.AuthToken)
                ? Fail($"'{nameof(SmartyStreetsAddressOptions)}' section is invalid")
                : Success;
        }
    }
}
