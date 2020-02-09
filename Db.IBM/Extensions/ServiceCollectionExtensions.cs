namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Configuration;
    using IBM.Data.DB2.Core;
    using JetBrains.Annotations;
    using Options;
    using Services;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds a hosted <see cref="IBMDbService"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddIBMDbService(
            this IServiceCollection services,
            Action<IBMDbOptions> configureOptions)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == default)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddSingleton<IValidateOptions<IBMDbOptions>, ValidateIBMDbOptions>();
            services.Configure(configureOptions);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<IBMDbOptions>>().Value;
                return services.AddIBMDbService(options);
            }
        }

        /// <summary>Adds a scoped <see cref="IBMDbService"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddIBMDbService(
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

            services.AddSingleton<IValidateOptions<IBMDbOptions>, ValidateIBMDbOptions>();
            services.Configure<IBMDbOptions>(config);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<IBMDbOptions>>().Value;
                return services.AddIBMDbService(options);
            }
        }

        /// <summary>Adds a scoped <see cref="IBMDbService"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddIBMDbService(
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

            services.AddSingleton<IValidateOptions<IBMDbOptions>, ValidateIBMDbOptions>();
            services.Configure<IBMDbOptions>(config, configureBinder);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<IBMDbOptions>>().Value;
                return services.AddIBMDbService(options);
            }
        }

        private static IServiceCollection AddIBMDbService(this IServiceCollection services, IBMDbOptions options)
        {
            services.AddSingleton(new DB2ConnectionStringBuilder
            {
                Database = options.Database,
                DBName = options.DBName,
                UserID = options.UserId,
                Password = options.Password
            });
            services.AddScoped<IBMDbService>();
            return services;
        }
    }
}
