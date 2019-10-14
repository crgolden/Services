namespace Services
{
    using System;
    using System.Collections.Generic;
    using Hangfire;
    using Hangfire.SqlServer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHangfireJobService(
            this IServiceCollection services,
            IConfiguration configuration,
            IEnumerable<HangfireJobDetail> hangfireJobDetails)
        {
            var section = configuration.GetSection(nameof(HangfireJobOptions));
            if (!section.Exists())
            {
                throw new Exception("HangfireJobOptions section doesn't exist");
            }

            services.Configure<HangfireJobOptions>(section);
            var hangfireJobOptions = section.Get<HangfireJobOptions>();
            if (hangfireJobOptions == default ||
                string.IsNullOrEmpty(hangfireJobOptions.ConnectionString))
            {
                throw new Exception("HangfireJobOptions section is invalid");
            }

            foreach (var hangfireJobDetail in hangfireJobDetails)
            {
                services.AddSingleton(hangfireJobDetail);
            }

            services.AddHangfire(options => options
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(
                        nameOrConnectionString: hangfireJobOptions.ConnectionString,
                        options: new SqlServerStorageOptions
                        {
                            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                            QueuePollInterval = TimeSpan.Zero,
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
