namespace Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;

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
                throw new Exception($"{nameof(QuartzJobStoreOptions)} section doesn't exist");
            }

            services.Configure<QuartzJobStoreOptions>(section);
            var quartzJobStoreOptions = section.Get<QuartzJobStoreOptions>();
            if (quartzJobStoreOptions == default ||
                string.IsNullOrEmpty(quartzJobStoreOptions.InstanceName) ||
                string.IsNullOrEmpty(quartzJobStoreOptions.InstanceId) ||
                string.IsNullOrEmpty(quartzJobStoreOptions.Type) ||
                string.IsNullOrEmpty(quartzJobStoreOptions.DriverDelegateType) ||
                string.IsNullOrEmpty(quartzJobStoreOptions.DataSource) ||
                string.IsNullOrEmpty(quartzJobStoreOptions.TablePrefix) ||
                string.IsNullOrEmpty(quartzJobStoreOptions.UseProperties) ||
                string.IsNullOrEmpty(quartzJobStoreOptions.LockHandlerType) ||
                string.IsNullOrEmpty(quartzJobStoreOptions.DataSourceProvider) ||
                string.IsNullOrEmpty(quartzJobStoreOptions.DataSourceConnectionString))
            {
                throw new Exception($"{nameof(QuartzJobStoreOptions)} section is invalid");
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
