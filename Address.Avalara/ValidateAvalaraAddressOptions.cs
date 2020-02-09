namespace Services
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    /// <inheritdoc />
    [PublicAPI]
    public class ValidateAvalaraAddressOptions : IValidateOptions<AvalaraAddressOptions>
    {
        /// <inheritdoc />
        public ValidateOptionsResult Validate(string name, AvalaraAddressOptions options)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return IsNullOrWhiteSpace(options.LicenseKey) ||
                   options.BaseAddress == default
                ? Fail($"'{nameof(AvalaraAddressOptions)}' section is invalid")
                : Success;
        }
    }
}
