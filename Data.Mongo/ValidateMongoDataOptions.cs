namespace Services
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Constants.ExceptionMessages;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    /// <inheritdoc />
    [PublicAPI]
    public class ValidateMongoDataOptions : IValidateOptions<MongoDataOptions>
    {
        /// <inheritdoc />
        public ValidateOptionsResult Validate(string name, MongoDataOptions options)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return IsNullOrWhiteSpace(options.DatabaseName) ? Fail(MissingDatabaseInfo(name)) : Success;
        }
    }
}
