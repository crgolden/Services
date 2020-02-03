namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Hangfire;
    using JetBrains.Annotations;
    using Options;
    using Services;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds a hosted <see cref="HangfireJobService"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <param name="hangfireJobDetails">The Hangfire job details.</param>
        /// <param name="configureGlobal">The configure global.</param>
        /// <param name="configureBackgroundJobServer">The configure background job server.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null"/>
        /// or
        /// <paramref name="hangfireJobDetails"/> is <see langword="null"/>
        /// or
        /// <paramref name="configureGlobal"/> is <see langword="null"/>.</exception>
        public static IServiceCollection AddHangfireJobService(
            this IServiceCollection services,
            Action<HangfireJobOptions> configureOptions,
            IEnumerable<HangfireJobDetail> hangfireJobDetails,
            Action<IGlobalConfiguration> configureGlobal,
            Action<BackgroundJobServerOptions> configureBackgroundJobServer = default)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == default)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            if (hangfireJobDetails == null)
            {
                throw new ArgumentNullException(nameof(hangfireJobDetails));
            }

            if (configureGlobal == default)
            {
                throw new ArgumentNullException(nameof(configureGlobal));
            }

            services.Configure(configureOptions);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<HangfireJobOptions>>().Value;
                return services.AddHangfireJobService(options, hangfireJobDetails, configureGlobal, configureBackgroundJobServer);
            }
        }

        /// <summary>Adds a hosted <see cref="HangfireJobService"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="hangfireJobDetails">The Hangfire job details.</param>
        /// <param name="configureGlobal">The configure global.</param>
        /// <param name="configureBackgroundJobServer">The configure background job server.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>
        /// or
        /// <paramref name="config"/> is <see langword="null"/>
        /// or
        /// <paramref name="hangfireJobDetails"/> is <see langword="null"/>
        /// or
        /// <paramref name="configureGlobal"/> is <see langword="null"/>.</exception>
        public static IServiceCollection AddHangfireJobService(
            this IServiceCollection services,
            IConfigurationSection config,
            IEnumerable<HangfireJobDetail> hangfireJobDetails,
            Action<IGlobalConfiguration> configureGlobal,
            Action<BackgroundJobServerOptions> configureBackgroundJobServer = default)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == default)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (hangfireJobDetails == null)
            {
                throw new ArgumentNullException(nameof(hangfireJobDetails));
            }

            if (configureGlobal == default)
            {
                throw new ArgumentNullException(nameof(configureGlobal));
            }

            services.Configure<HangfireJobOptions>(config);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<HangfireJobOptions>>().Value;
                return services.AddHangfireJobService(options, hangfireJobDetails, configureGlobal, configureBackgroundJobServer);
            }
        }

        /// <summary>Adds a hosted <see cref="HangfireJobService"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <param name="hangfireJobDetails">The Hangfire job details.</param>
        /// <param name="configureGlobal">The configure global.</param>
        /// <param name="configureBackgroundJobServer">The configure background job server.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>
        /// or
        /// <paramref name="config"/> is <see langword="null"/>
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null"/>
        /// or
        /// <paramref name="hangfireJobDetails"/> is <see langword="null"/>
        /// or
        /// <paramref name="configureGlobal"/> is <see langword="null"/>.</exception>
        public static IServiceCollection AddHangfireJobService(
            this IServiceCollection services,
            IConfigurationSection config,
            Action<BinderOptions> configureBinder,
            IEnumerable<HangfireJobDetail> hangfireJobDetails,
            Action<IGlobalConfiguration> configureGlobal,
            Action<BackgroundJobServerOptions> configureBackgroundJobServer = default)
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

            if (hangfireJobDetails == null)
            {
                throw new ArgumentNullException(nameof(hangfireJobDetails));
            }

            if (configureGlobal == default)
            {
                throw new ArgumentNullException(nameof(configureGlobal));
            }

            services.Configure<HangfireJobOptions>(config, configureBinder);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<HangfireJobOptions>>().Value;
                return services.AddHangfireJobService(options, hangfireJobDetails, configureGlobal, configureBackgroundJobServer);
            }
        }

        private static IServiceCollection AddHangfireJobService(
            this IServiceCollection services,
            HangfireJobOptions options,
            IEnumerable<HangfireJobDetail> hangfireJobDetails,
            Action<IGlobalConfiguration> configureGlobal,
            Action<BackgroundJobServerOptions> configureBackgroundJobServer = default)
        {
            foreach (var hangfireJobDetail in hangfireJobDetails)
            {
                services.AddSingleton(hangfireJobDetail);
            }

            services.AddHangfire(configureGlobal);
            services.AddHostedService<HangfireJobService>();
            if (!options.UseHangfireServer)
            {
                return services;
            }

            if (configureBackgroundJobServer != default)
            {
                services.AddHangfireServer(configureBackgroundJobServer);
            }
            else
            {
                services.AddHangfireServer();
            }

            return services;
        }
    }
}
