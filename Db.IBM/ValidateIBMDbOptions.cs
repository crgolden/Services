namespace Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    /// <inheritdoc />
    [PublicAPI]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "IBM is an abbreviation")]
    public class ValidateIBMDbOptions : IValidateOptions<IBMDbOptions>
    {
        /// <inheritdoc />
        public ValidateOptionsResult Validate(string name, IBMDbOptions options)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return IsNullOrWhiteSpace(options.Database) ||
                   IsNullOrWhiteSpace(options.DBName) ||
                   IsNullOrWhiteSpace(options.UserId) ||
                   IsNullOrWhiteSpace(options.Password)
                ? Fail($"'{nameof(IBMDbOptions)}' section is invalid")
                : Success;
        }
    }
}
