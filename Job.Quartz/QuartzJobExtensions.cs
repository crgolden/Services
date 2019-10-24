namespace Services
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;
    using static System.Reflection.Assembly;
    using static QuartzJobStore;

    public static class QuartzJobExtensions
    {
        public static IServiceCollection AddQuartzJobService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var schedulerFactory = new StdSchedulerFactory();
            var section = configuration.GetSection(nameof(QuartzJobStoreOptions));
            if (section.Exists())
            {
                var options = section.Get<QuartzJobStoreOptions>();
                var props = JobStoreProps(options);
                schedulerFactory.Initialize(props);
            }

            services.AddSingleton<IJobFactory, QuartzJobFactory>();
            services.AddSingleton<ISchedulerFactory>(schedulerFactory);
            foreach (var job in GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(QuartzJobDetail)) && !x.IsAbstract))
            {
                services.AddSingleton(job);
            }

            return services;
        }
    }
}
