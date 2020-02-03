namespace Services.Options
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used implicitly")]
    internal class ValidateOptions : IValidateOptions<SqlDbOptions>
    {
        public ValidateOptionsResult Validate(string name, SqlDbOptions options)
        {
            return IsNullOrWhiteSpace(options.DataSource) ||
                   IsNullOrWhiteSpace(options.UserId) ||
                   IsNullOrWhiteSpace(options.Password)
                ? Fail($"'{nameof(SqlDbOptions)}' section is invalid")
                : Success;
        }
    }
}
