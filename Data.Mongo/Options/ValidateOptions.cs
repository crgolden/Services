namespace Services.Options
{
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Constants.ExceptionMessages;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    [UsedImplicitly]
    internal class ValidateOptions : IValidateOptions<MongoDataOptions>
    {
        public ValidateOptionsResult Validate(string name, MongoDataOptions options)
        {
            var failures = new List<string>();
            if (IsNullOrEmpty(options.DatabaseName) || IsNullOrWhiteSpace(options.DatabaseName))
            {
                failures.Add(MissingDatabaseInfo(name));
            }

            return failures.Any() ? Fail(failures) : Success;
        }
    }
}
