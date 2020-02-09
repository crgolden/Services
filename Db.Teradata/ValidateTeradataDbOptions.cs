namespace Services
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    /// <inheritdoc />
    [PublicAPI]
    public class ValidateTeradataDbOptions : IValidateOptions<TeradataDbOptions>
    {
        /// <inheritdoc />
        public ValidateOptionsResult Validate(string name, TeradataDbOptions options)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

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
