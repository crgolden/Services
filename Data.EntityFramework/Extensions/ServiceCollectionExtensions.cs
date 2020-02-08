namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Configuration;
    using EntityFrameworkCore;
    using JetBrains.Annotations;
    using Options;
    using Services;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds a scoped <see cref="EntityFrameworkDataService"/> to <paramref name="services"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <typeparam name="T">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            Action<EntityFrameworkDataOptions> configureOptions,
            Action<DbContextOptionsBuilder> optionsAction = default)
            where T : DbContext
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == default)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.Configure(configureOptions);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetService<IOptions<EntityFrameworkDataOptions>>().Value;
                return services.AddEntityFrameworkDataService<T>(options, optionsAction);
            }
        }

        /// <summary>Adds a scoped <see cref="EntityFrameworkDataService"/> to <paramref name="services"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <typeparam name="T">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            IConfigurationSection config,
            Action<DbContextOptionsBuilder> optionsAction = default)
            where T : DbContext
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == default)
            {
                throw new ArgumentNullException(nameof(config));
            }

            services.Configure<EntityFrameworkDataOptions>(config);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetService<IOptions<EntityFrameworkDataOptions>>().Value;
                return services.AddEntityFrameworkDataService<T>(options, optionsAction);
            }
        }

        /// <summary>Adds a scoped <see cref="EntityFrameworkDataService"/> to <paramref name="services"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <typeparam name="T">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            IConfigurationSection config,
            Action<BinderOptions> configureBinder,
            Action<DbContextOptionsBuilder> optionsAction = default)
            where T : DbContext
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

            services.Configure<EntityFrameworkDataOptions>(config, configureBinder);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetService<IOptions<EntityFrameworkDataOptions>>().Value;
                return services.AddEntityFrameworkDataService<T>(options, optionsAction);
            }
        }

        private static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            EntityFrameworkDataOptions options,
            Action<DbContextOptionsBuilder> optionsAction)
            where T : DbContext
        {
            services.AddScoped<DbContext, T>();
            if (options == default)
            {
                return services.AddDbContext<T>(optionsAction);
            }

            if (options.UsePooling && optionsAction != default)
            {
                return options.PoolSize.HasValue
                    ? services.AddDbContextPool<T>(optionsAction, options.PoolSize.Value)
                    : services.AddDbContextPool<T>(optionsAction);
            }

            return services.AddDbContext<T>(optionsAction);
        }
    }
}
