namespace Services.Options
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used implicitly")]
    internal class ValidateOptions : IValidateOptions<EmailQueueClientOptions>
    {
        public ValidateOptionsResult Validate(string name, EmailQueueClientOptions options)
        {
            return IsNullOrWhiteSpace(options.EmailQueueName) ||
                   IsNullOrWhiteSpace(options.Endpoint) ||
                   IsNullOrWhiteSpace(options.SharedAccessKeyName) ||
                   (IsNullOrWhiteSpace(options.PrimaryKey) && IsNullOrWhiteSpace(options.SecondaryKey))
                ? Fail($"'{nameof(EmailQueueClientOptions)}' section is invalid")
                : Success;
        }
    }
}
