namespace Services.Options
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used implicitly")]
    internal class ValidateOptions : IValidateOptions<StripePaymentOptions>
    {
        public ValidateOptionsResult Validate(string name, StripePaymentOptions options)
        {
            return IsNullOrWhiteSpace(options.SecretKey) ? Fail($"'{nameof(StripePaymentOptions)}' section is invalid") : Success;
        }
    }
}
