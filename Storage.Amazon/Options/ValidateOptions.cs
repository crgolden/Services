namespace Services.Options
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used implicitly")]
    internal class ValidateOptions : IValidateOptions<AmazonStorageOptions>
    {
        public ValidateOptionsResult Validate(string name, AmazonStorageOptions options)
        {
            return IsNullOrWhiteSpace(options.AccessKeyId) || IsNullOrWhiteSpace(options.SecretAccessKey)
                ? Fail($"'{nameof(AmazonStorageOptions)}' section is invalid")
                : Success;
        }
    }
}
