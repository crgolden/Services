namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Configuration;
    using JetBrains.Annotations;
    using Options;
    using Services;
    using Teradata.Client.Provider;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds a hosted <see cref="TeradataDbService"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddTeradataDbService(
            this IServiceCollection services,
            Action<TeradataDbOptions> configureOptions)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == default)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddSingleton<IValidateOptions<TeradataDbOptions>, ValidateTeradataDbOptions>();
            services.Configure(configureOptions);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<TeradataDbOptions>>().Value;
                return services.AddTeradataDbService(options);
            }
        }

        /// <summary>Adds a scoped <see cref="TeradataDbService"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddTeradataDbService(
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

            services.AddSingleton<IValidateOptions<TeradataDbOptions>, ValidateTeradataDbOptions>();
            services.Configure<TeradataDbOptions>(config);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<TeradataDbOptions>>().Value;
                return services.AddTeradataDbService(options);
            }
        }

        /// <summary>Adds a scoped <see cref="TeradataDbService"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddTeradataDbService(
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

            services.AddSingleton<IValidateOptions<TeradataDbOptions>, ValidateTeradataDbOptions>();
            services.Configure<TeradataDbOptions>(config, configureBinder);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<TeradataDbOptions>>().Value;
                return services.AddTeradataDbService(options);
            }
        }

        private static IServiceCollection AddTeradataDbService(this IServiceCollection services, TeradataDbOptions options)
        {
            services.AddSingleton(new TdConnectionStringBuilder
            {
                DataSource = options.DataSource,
                Database = options.Database,
                UserId = options.UserId,
                Password = options.Password,
                AuthenticationMechanism = options.AuthenticationMechanism
            });
            services.AddScoped<TeradataDbService>();
            return services;
        }
    }
}
