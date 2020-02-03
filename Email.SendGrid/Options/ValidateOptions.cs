namespace Services.Options
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used implicitly")]
    internal class ValidateOptions : IValidateOptions<SendGridEmailOptions>
    {
        public ValidateOptionsResult Validate(string name, SendGridEmailOptions options)
        {
            return IsNullOrWhiteSpace(options.ApiKey) ? Fail($"'{nameof(SendGridEmailOptions)}' section is invalid") : Success;
        }
    }
}
