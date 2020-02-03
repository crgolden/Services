namespace Services.Options
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used implicitly")]
    internal class ValidateOptions : IValidateOptions<IBMDbOptions>
    {
        public ValidateOptionsResult Validate(string name, IBMDbOptions options)
        {
            return IsNullOrWhiteSpace(options.Database) ||
                   IsNullOrWhiteSpace(options.DBName) ||
                   IsNullOrWhiteSpace(options.UserId) ||
                   IsNullOrWhiteSpace(options.Password)
                ? Fail($"'{nameof(IBMDbOptions)}' section is invalid")
                : Success;
        }
    }
}
