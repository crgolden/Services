namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Configuration;
    using EntityFrameworkCore;
    using JetBrains.Annotations;
    using Options;
    using Services;
    using static ServiceLifetime;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds a scoped <see cref="EntityFrameworkDataService"/> to <paramref name="services"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <param name="contextLifetime"> The lifetime with which to register the <see cref="DbContext"/> service in the container. </param>
        /// <param name="optionsLifetime"> The lifetime with which to register the <see cref="DbContextOptions"/> service in the container. </param>
        /// <typeparam name="T">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            Action<EntityFrameworkDataOptions> configureOptions,
            Action<DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = Scoped,
            ServiceLifetime optionsLifetime = Scoped)
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

            if (optionsAction == default)
            {
                throw new ArgumentNullException(nameof(optionsAction));
            }

            services.Configure(configureOptions);
            EntityFrameworkDataOptions options;
            using (var provider = services.BuildServiceProvider(true))
            {
                options = provider.GetRequiredService<IOptions<EntityFrameworkDataOptions>>().Value;
            }

            return services.AddEntityFrameworkDataService<T>(options, optionsAction, contextLifetime, optionsLifetime);
        }

        /// <summary>Adds a scoped <see cref="EntityFrameworkDataService"/> to <paramref name="services"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <param name="contextLifetime"> The lifetime with which to register the <see cref="DbContext"/> service in the container. </param>
        /// <param name="optionsLifetime"> The lifetime with which to register the <see cref="DbContextOptions"/> service in the container. </param>
        /// <typeparam name="T">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            Action<EntityFrameworkDataOptions> configureOptions,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = Scoped,
            ServiceLifetime optionsLifetime = Scoped)
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

            if (optionsAction == default)
            {
                throw new ArgumentNullException(nameof(optionsAction));
            }

            services.Configure(configureOptions);
            EntityFrameworkDataOptions options;
            using (var provider = services.BuildServiceProvider(true))
            {
                options = provider.GetRequiredService<IOptions<EntityFrameworkDataOptions>>().Value;
            }

            return services.AddEntityFrameworkDataService<T>(options, optionsAction, contextLifetime, optionsLifetime);
        }

        /// <summary>Adds a scoped <see cref="EntityFrameworkDataService"/> to <paramref name="services"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <param name="contextLifetime"> The lifetime with which to register the <see cref="DbContext"/> service in the container. </param>
        /// <param name="optionsLifetime"> The lifetime with which to register the <see cref="DbContextOptions"/> service in the container. </param>
        /// <typeparam name="T">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            IConfigurationSection config,
            Action<DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = Scoped,
            ServiceLifetime optionsLifetime = Scoped)
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

            if (optionsAction == default)
            {
                throw new ArgumentNullException(nameof(optionsAction));
            }

            services.Configure<EntityFrameworkDataOptions>(config);
            EntityFrameworkDataOptions options;
            using (var provider = services.BuildServiceProvider(true))
            {
                options = provider.GetRequiredService<IOptions<EntityFrameworkDataOptions>>().Value;
            }

            return services.AddEntityFrameworkDataService<T>(options, optionsAction, contextLifetime, optionsLifetime);
        }

        /// <summary>Adds a scoped <see cref="EntityFrameworkDataService"/> to <paramref name="services"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <param name="contextLifetime"> The lifetime with which to register the <see cref="DbContext"/> service in the container. </param>
        /// <param name="optionsLifetime"> The lifetime with which to register the <see cref="DbContextOptions"/> service in the container. </param>
        /// <typeparam name="T">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            IConfigurationSection config,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = Scoped,
            ServiceLifetime optionsLifetime = Scoped)
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

            if (optionsAction == default)
            {
                throw new ArgumentNullException(nameof(optionsAction));
            }

            services.Configure<EntityFrameworkDataOptions>(config);
            EntityFrameworkDataOptions options;
            using (var provider = services.BuildServiceProvider(true))
            {
                options = provider.GetRequiredService<IOptions<EntityFrameworkDataOptions>>().Value;
            }

            return services.AddEntityFrameworkDataService<T>(options, optionsAction, contextLifetime, optionsLifetime);
        }

        /// <summary>Adds a scoped <see cref="EntityFrameworkDataService"/> to <paramref name="services"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <param name="contextLifetime"> The lifetime with which to register the <see cref="DbContext"/> service in the container. </param>
        /// <param name="optionsLifetime"> The lifetime with which to register the <see cref="DbContextOptions"/> service in the container. </param>
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
            Action<DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = Scoped,
            ServiceLifetime optionsLifetime = Scoped)
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

            if (optionsAction == default)
            {
                throw new ArgumentNullException(nameof(optionsAction));
            }

            services.Configure<EntityFrameworkDataOptions>(config, configureBinder);
            EntityFrameworkDataOptions options;
            using (var provider = services.BuildServiceProvider(true))
            {
                options = provider.GetRequiredService<IOptions<EntityFrameworkDataOptions>>().Value;
            }

            return services.AddEntityFrameworkDataService<T>(options, optionsAction, contextLifetime, optionsLifetime);
        }

        /// <summary>Adds a scoped <see cref="EntityFrameworkDataService"/> to <paramref name="services"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <param name="contextLifetime"> The lifetime with which to register the <see cref="DbContext"/> service in the container. </param>
        /// <param name="optionsLifetime"> The lifetime with which to register the <see cref="DbContextOptions"/> service in the container. </param>
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
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = Scoped,
            ServiceLifetime optionsLifetime = Scoped)
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

            if (optionsAction == default)
            {
                throw new ArgumentNullException(nameof(optionsAction));
            }

            services.Configure<EntityFrameworkDataOptions>(config, configureBinder);
            EntityFrameworkDataOptions options;
            using (var provider = services.BuildServiceProvider(true))
            {
                options = provider.GetRequiredService<IOptions<EntityFrameworkDataOptions>>().Value;
            }

            return services.AddEntityFrameworkDataService<T>(options, optionsAction, contextLifetime, optionsLifetime);
        }

        private static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            EntityFrameworkDataOptions options,
            Action<DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime,
            ServiceLifetime optionsLifetime)
            where T : DbContext
        {
            if (options.UsePooling)
            {
                if (options.PoolSize.HasValue)
                {
                    services.AddDbContextPool<T>(optionsAction, options.PoolSize.Value);
                }
                else
                {
                    services.AddDbContextPool<T>(optionsAction);
                }
            }
            else
            {
                services.AddDbContext<T>(optionsAction, contextLifetime, optionsLifetime);
            }

            var descriptor = new ServiceDescriptor(typeof(DbContext), typeof(T), contextLifetime);
            services.Add(descriptor);
            return services;
        }

        private static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            EntityFrameworkDataOptions options,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime,
            ServiceLifetime optionsLifetime)
            where T : DbContext
        {
            if (options.UsePooling)
            {
                if (options.PoolSize.HasValue)
                {
                    services.AddDbContextPool<T>(optionsAction, options.PoolSize.Value);
                }
                else
                {
                    services.AddDbContextPool<T>(optionsAction);
                }
            }
            else
            {
                services.AddDbContext<T>(optionsAction, contextLifetime, optionsLifetime);
            }

            var descriptor = new ServiceDescriptor(typeof(DbContext), typeof(T), contextLifetime);
            services.Add(descriptor);
            return services;
        }
    }
}
