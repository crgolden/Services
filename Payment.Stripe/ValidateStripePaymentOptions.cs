namespace Services
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    /// <inheritdoc />
    [PublicAPI]
    public class ValidateStripePaymentOptions : IValidateOptions<StripePaymentOptions>
    {
        /// <inheritdoc />
        public ValidateOptionsResult Validate(string name, StripePaymentOptions options)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return IsNullOrWhiteSpace(options.SecretKey)
                ? Fail($"'{nameof(StripePaymentOptions)}' section is invalid")
                : Success;
        }
    }
}
