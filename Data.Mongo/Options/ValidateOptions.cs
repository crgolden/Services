﻿namespace Services.Options
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Constants.ExceptionMessages;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used implicitly")]
    internal class ValidateOptions : IValidateOptions<MongoDataOptions>
    {
        public ValidateOptionsResult Validate(string name, MongoDataOptions options)
        {
            return IsNullOrWhiteSpace(options.DatabaseName) ? Fail(MissingDatabaseInfo(name)) : Success;
        }
    }
}
