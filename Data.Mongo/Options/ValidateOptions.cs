namespace Services.Options
{
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
            return IsNullOrWhiteSpace(options.DatabaseName) ? Fail(MissingDatabaseInfo(name)) : Success;
        }
    }
}
