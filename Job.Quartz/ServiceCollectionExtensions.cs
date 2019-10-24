namespace Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;
    using static System.String;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQuartzJobService(
            this IServiceCollection services,
            IConfiguration? configuration,
            IEnumerable<IJob>? quartzJobDetails)
        {
            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (quartzJobDetails == null)
            {
                throw new ArgumentNullException(nameof(quartzJobDetails));
            }

            var section = configuration.GetSection(nameof(QuartzJobStoreOptions));
            if (!section.Exists())
            {
                throw new ArgumentException(
                    message: $"{nameof(QuartzJobStoreOptions)} section doesn't exist",
                    paramName: nameof(configuration));
            }

            services.Configure<QuartzJobStoreOptions>(section);
            var options = section.Get<QuartzJobStoreOptions>();
            if (options == default ||
                IsNullOrEmpty(options.InstanceName) ||
                IsNullOrEmpty(options.InstanceId) ||
                IsNullOrEmpty(options.Type) ||
                IsNullOrEmpty(options.DriverDelegateType) ||
                IsNullOrEmpty(options.DataSource) ||
                IsNullOrEmpty(options.TablePrefix) ||
                IsNullOrEmpty(options.UseProperties) ||
                IsNullOrEmpty(options.LockHandlerType) ||
                IsNullOrEmpty(options.DataSourceProvider) ||
                IsNullOrEmpty(options.DataSourceConnectionString))
            {
                throw new ArgumentException(
                    message: $"{nameof(QuartzJobStoreOptions)} section is invalid",
                    paramName: nameof(configuration));
            }

            foreach (var quartzJobDetail in quartzJobDetails)
            {
                services.AddSingleton(quartzJobDetail);
            }

            services.AddSingleton<IJobFactory, QuartzJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddHostedService<QuartzJobService>();
            return services;
        }
    }
}
