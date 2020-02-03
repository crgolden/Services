namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Data.SqlClient;
    using Configuration;
    using JetBrains.Annotations;
    using Options;
    using Services;
    using Services.Options;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds a hosted <see cref="SqlDbService"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddSqlDbService(
            this IServiceCollection services,
            Action<SqlDbOptions> configureOptions)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == default)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddSingleton<IValidateOptions<SqlDbOptions>, ValidateOptions>();
            services.Configure(configureOptions);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<SqlDbOptions>>().Value;
                return services.AddSqlDbService(options);
            }
        }

        /// <summary>Adds a scoped <see cref="SqlDbService"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddSqlDbService(
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

            services.AddSingleton<IValidateOptions<SqlDbOptions>, ValidateOptions>();
            services.Configure<SqlDbOptions>(config);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<SqlDbOptions>>().Value;
                return services.AddSqlDbService(options);
            }
        }

        /// <summary>Adds a scoped <see cref="SqlDbService"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddSqlDbService(
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

            services.AddSingleton<IValidateOptions<SqlDbOptions>, ValidateOptions>();
            services.Configure<SqlDbOptions>(config, configureBinder);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<SqlDbOptions>>().Value;
                return services.AddSqlDbService(options);
            }
        }

        private static IServiceCollection AddSqlDbService(this IServiceCollection services, SqlDbOptions options)
        {
            services.AddSingleton(new SqlConnectionStringBuilder
            {
                DataSource = options.DataSource,
                UserID = options.UserId,
                Password = options.Password
            });
            services.AddScoped<SqlDbService>();
            return services;
        }
    }
}
