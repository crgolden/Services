namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using JetBrains.Annotations;
    using Options;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;
    using Services;
    using static System.Activator;
    using static System.Reflection.Assembly;
    using static Services.QuartzJobStore;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds a hosted <see cref="QuartzJobService"/> using the provided <paramref name="quartzJobDetails"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="quartzJobDetails">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="quartzJobDetails"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddQuartzJobService(this IServiceCollection services, IEnumerable<IJob> quartzJobDetails)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (quartzJobDetails == null)
            {
                throw new ArgumentNullException(nameof(quartzJobDetails));
            }

            foreach (var quartzJobDetail in quartzJobDetails)
            {
                services.AddSingleton(quartzJobDetail);
            }

            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<IJobFactory, QuartzJobFactory>();
            services.AddHostedService<QuartzJobService>();
            return services;
        }

        /// <summary>Adds a hosted <see cref="QuartzJobService"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddQuartzJobService(
            this IServiceCollection services,
            Action<QuartzJobStoreOptions> configureOptions)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == default)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddSingleton<IValidateOptions<QuartzJobStoreOptions>, ValidateQuartzJobStoreOptions>();
            services.Configure(configureOptions);
            using var provider = services.BuildServiceProvider(true);
            var options = provider.GetRequiredService<IOptions<QuartzJobStoreOptions>>().Value;
            return services.AddQuartzJobService(options);
        }

        /// <summary>Adds a hosted <see cref="QuartzJobService"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddQuartzJobService(
            this IServiceCollection services,
            IConfigurationSection config)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == default)
            {
                throw new ArgumentNullException(nameof(config));
            }

            services.AddSingleton<IValidateOptions<QuartzJobStoreOptions>, ValidateQuartzJobStoreOptions>();
            services.Configure<QuartzJobStoreOptions>(config);
            using var provider = services.BuildServiceProvider(true);
            var options = provider.GetRequiredService<IOptions<QuartzJobStoreOptions>>().Value;
            return services.AddQuartzJobService(options);
        }

        /// <summary>Adds a hosted <see cref="QuartzJobService"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddQuartzJobService(
            this IServiceCollection services,
            IConfigurationSection config,
            Action<BinderOptions> configureBinder)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == default)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (configureBinder == default)
            {
                throw new ArgumentNullException(nameof(configureBinder));
            }

            services.AddSingleton<IValidateOptions<QuartzJobStoreOptions>, ValidateQuartzJobStoreOptions>();
            services.Configure<QuartzJobStoreOptions>(config, configureBinder);
            using var provider = services.BuildServiceProvider(true);
            var options = provider.GetRequiredService<IOptions<QuartzJobStoreOptions>>().Value;
            return services.AddQuartzJobService(options);
        }

        private static IServiceCollection AddQuartzJobService(this IServiceCollection services, QuartzJobStoreOptions options)
        {
            foreach (var type in GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(QuartzJobDetail)) && !x.IsAbstract))
            {
                var job = CreateInstance(type);
                services.AddSingleton((job as IJob)!);
            }

            var schedulerFactory = new StdSchedulerFactory();
            var props = JobStoreProps(options);
            schedulerFactory.Initialize(props);
            services.AddSingleton<ISchedulerFactory>(schedulerFactory);
            services.AddSingleton<IJobFactory, QuartzJobFactory>();
            services.AddHostedService<QuartzJobService>();
            return services;
        }
    }
}
