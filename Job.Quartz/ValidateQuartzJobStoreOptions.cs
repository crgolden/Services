namespace Services
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    /// <inheritdoc />
    [PublicAPI]
    public class ValidateQuartzJobStoreOptions : IValidateOptions<QuartzJobStoreOptions>
    {
        /// <inheritdoc />
        public ValidateOptionsResult Validate(string name, QuartzJobStoreOptions options)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

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
