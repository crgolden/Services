namespace Services
{
    using System;
    using System.Collections.Generic;
    using Hangfire;
    using Hangfire.SqlServer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using static System.String;
    using static System.TimeSpan;
    using static Hangfire.CompatibilityLevel;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHangfireJobService(
            this IServiceCollection services,
            IConfiguration? configuration,
            IEnumerable<HangfireJobDetail>? hangfireJobDetails)
        {
            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (hangfireJobDetails == null)
            {
                throw new ArgumentNullException(nameof(hangfireJobDetails));
            }

            var section = configuration.GetSection(nameof(HangfireJobOptions));
            if (!section.Exists())
            {
                throw new ArgumentException(
                    message: $"{nameof(HangfireJobOptions)} section doesn't exist",
                    paramName: nameof(configuration));
            }

            services.Configure<HangfireJobOptions>(section);
            var options = section.Get<HangfireJobOptions>();
            if (options == default ||
                IsNullOrEmpty(options.ConnectionString))
            {
                throw new ArgumentException(
                    message: $"{nameof(HangfireJobOptions)} section is invalid",
                    paramName: nameof(configuration));
            }

            foreach (var hangfireJobDetail in hangfireJobDetails)
            {
                services.AddSingleton(hangfireJobDetail);
            }

            services.AddHangfire(globalConfiguration => globalConfiguration
                    .SetDataCompatibilityLevel(Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(
                        nameOrConnectionString: options.ConnectionString,
                        options: new SqlServerStorageOptions
                        {
                            CommandBatchMaxTimeout = FromMinutes(5),
                            SlidingInvisibilityTimeout = FromMinutes(5),
                            QueuePollInterval = Zero,
                            UseRecommendedIsolationLevel = true,
                            UsePageLocksOnDequeue = true,
                            DisableGlobalLocks = true
                        }));
            services.AddHangfireServer();
            services.AddHostedService<HangfireJobService>();
            return services;
        }
    }
}
