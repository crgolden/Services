namespace Services.Options
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used implicitly")]
    internal class ValidateOptions : IValidateOptions<AzureStorageOptions>
    {
        public ValidateOptionsResult Validate(string name, AzureStorageOptions options)
        {
            return IsNullOrWhiteSpace(options.AccountName) ||
            (IsNullOrWhiteSpace(options.AccountKey1) && IsNullOrWhiteSpace(options.AccountKey2))
                ? Fail($"'{nameof(AzureStorageOptions)}' section is invalid")
                : Success;
        }
    }
}
