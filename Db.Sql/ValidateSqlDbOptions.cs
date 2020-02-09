namespace Services
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    /// <inheritdoc />
    [PublicAPI]
    public class ValidateSqlDbOptions : IValidateOptions<SqlDbOptions>
    {
        /// <inheritdoc />
        public ValidateOptionsResult Validate(string name, SqlDbOptions options)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return IsNullOrWhiteSpace(options.DataSource) ||
                   IsNullOrWhiteSpace(options.UserId) ||
                   IsNullOrWhiteSpace(options.Password)
                ? Fail($"'{nameof(SqlDbOptions)}' section is invalid")
                : Success;
        }
    }
}
