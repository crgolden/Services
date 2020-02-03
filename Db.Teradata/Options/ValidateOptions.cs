namespace Services.Options
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used implicitly")]
    internal class ValidateOptions : IValidateOptions<TeradataDbOptions>
    {
        public ValidateOptionsResult Validate(string name, TeradataDbOptions options)
        {
            return IsNullOrWhiteSpace(options.DataSource) ||
                   IsNullOrWhiteSpace(options.Database) ||
                   IsNullOrWhiteSpace(options.UserId) ||
                   IsNullOrWhiteSpace(options.Password) ||
                   IsNullOrWhiteSpace(options.AuthenticationMechanism)
                ? Fail($"'{nameof(TeradataDbOptions)}' section is invalid")
                : Success;
        }
    }
}
