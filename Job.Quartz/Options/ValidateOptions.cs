namespace Services.Options
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used implicitly")]
    internal class ValidateOptions : IValidateOptions<QuartzJobStoreOptions>
    {
        public ValidateOptionsResult Validate(string name, QuartzJobStoreOptions options)
        {
            return IsNullOrWhiteSpace(options.InstanceName) ||
                   IsNullOrWhiteSpace(options.InstanceId) ||
                   IsNullOrWhiteSpace(options.Type) ||
                   IsNullOrWhiteSpace(options.DriverDelegateType) ||
                   IsNullOrWhiteSpace(options.DataSource) ||
                   IsNullOrWhiteSpace(options.TablePrefix) ||
                   IsNullOrWhiteSpace(options.UseProperties) ||
                   IsNullOrWhiteSpace(options.LockHandlerType) ||
                   IsNullOrWhiteSpace(options.DataSourceProvider) ||
                   IsNullOrWhiteSpace(options.DataSourceConnectionString)
                ? Fail($"'{nameof(QuartzJobStoreOptions)}' section is invalid")
                : Success;
        }
    }
}
