namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Common.Services;
    using Configuration;
    using EntityFrameworkCore;
    using JetBrains.Annotations;
    using Options;
    using Services;
    using static System.String;
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
        /// <param name="name">The name.</param>
        /// <typeparam name="T">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            Action<EntityFrameworkDataOptions> configureOptions,
            Action<DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = Scoped,
            ServiceLifetime optionsLifetime = Scoped,
            string name = nameof(EntityFrameworkDataService))
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

            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            services.Configure(configureOptions);
            EntityFrameworkDataOptions options;
            using (var provider = services.BuildServiceProvider(true))
            {
                options = provider.GetRequiredService<IOptions<EntityFrameworkDataOptions>>().Value;
            }

            return services.AddEntityFrameworkDataService<T>(options, optionsAction, contextLifetime, optionsLifetime, name);
        }

        /// <summary>Adds a scoped <see cref="EntityFrameworkDataService"/> to <paramref name="services"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <param name="contextLifetime"> The lifetime with which to register the <see cref="DbContext"/> service in the container. </param>
        /// <param name="optionsLifetime"> The lifetime with which to register the <see cref="DbContextOptions"/> service in the container. </param>
        /// <param name="name">The name.</param>
        /// <typeparam name="T">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            Action<EntityFrameworkDataOptions> configureOptions,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = Scoped,
            ServiceLifetime optionsLifetime = Scoped,
            string name = nameof(EntityFrameworkDataService))
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

            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            services.Configure(configureOptions);
            EntityFrameworkDataOptions options;
            using (var provider = services.BuildServiceProvider(true))
            {
                options = provider.GetRequiredService<IOptions<EntityFrameworkDataOptions>>().Value;
            }

            return services.AddEntityFrameworkDataService<T>(options, optionsAction, contextLifetime, optionsLifetime, name);
        }

        /// <summary>Adds a scoped <see cref="EntityFrameworkDataService"/> to <paramref name="services"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <param name="contextLifetime"> The lifetime with which to register the <see cref="DbContext"/> service in the container. </param>
        /// <param name="optionsLifetime"> The lifetime with which to register the <see cref="DbContextOptions"/> service in the container. </param>
        /// <param name="name">The name.</param>
        /// <typeparam name="T">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            IConfigurationSection config,
            Action<DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = Scoped,
            ServiceLifetime optionsLifetime = Scoped,
            string name = nameof(EntityFrameworkDataService))
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

            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            services.Configure<EntityFrameworkDataOptions>(config);
            EntityFrameworkDataOptions options;
            using (var provider = services.BuildServiceProvider(true))
            {
                options = provider.GetRequiredService<IOptions<EntityFrameworkDataOptions>>().Value;
            }

            return services.AddEntityFrameworkDataService<T>(options, optionsAction, contextLifetime, optionsLifetime, name);
        }

        /// <summary>Adds a scoped <see cref="EntityFrameworkDataService"/> to <paramref name="services"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <param name="contextLifetime"> The lifetime with which to register the <see cref="DbContext"/> service in the container. </param>
        /// <param name="optionsLifetime"> The lifetime with which to register the <see cref="DbContextOptions"/> service in the container. </param>
        /// <param name="name">The name.</param>
        /// <typeparam name="T">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            IConfigurationSection config,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = Scoped,
            ServiceLifetime optionsLifetime = Scoped,
            string name = nameof(EntityFrameworkDataService))
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

            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            services.Configure<EntityFrameworkDataOptions>(config);
            EntityFrameworkDataOptions options;
            using (var provider = services.BuildServiceProvider(true))
            {
                options = provider.GetRequiredService<IOptions<EntityFrameworkDataOptions>>().Value;
            }

            return services.AddEntityFrameworkDataService<T>(options, optionsAction, contextLifetime, optionsLifetime, name);
        }

        /// <summary>Adds a scoped <see cref="EntityFrameworkDataService"/> to <paramref name="services"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <param name="contextLifetime"> The lifetime with which to register the <see cref="DbContext"/> service in the container. </param>
        /// <param name="optionsLifetime"> The lifetime with which to register the <see cref="DbContextOptions"/> service in the container. </param>
        /// <param name="name">The name.</param>
        /// <typeparam name="T">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            IConfigurationSection config,
            Action<BinderOptions> configureBinder,
            Action<DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = Scoped,
            ServiceLifetime optionsLifetime = Scoped,
            string name = nameof(EntityFrameworkDataService))
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

            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            services.Configure<EntityFrameworkDataOptions>(config, configureBinder);
            EntityFrameworkDataOptions options;
            using (var provider = services.BuildServiceProvider(true))
            {
                options = provider.GetRequiredService<IOptions<EntityFrameworkDataOptions>>().Value;
            }

            return services.AddEntityFrameworkDataService<T>(options, optionsAction, contextLifetime, optionsLifetime, name);
        }

        /// <summary>Adds a scoped <see cref="EntityFrameworkDataService"/> to <paramref name="services"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <param name="contextLifetime"> The lifetime with which to register the <see cref="DbContext"/> service in the container. </param>
        /// <param name="optionsLifetime"> The lifetime with which to register the <see cref="DbContextOptions"/> service in the container. </param>
        /// <param name="name">The name.</param>
        /// <typeparam name="T">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            IConfigurationSection config,
            Action<BinderOptions> configureBinder,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = Scoped,
            ServiceLifetime optionsLifetime = Scoped,
            string name = nameof(EntityFrameworkDataService))
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

            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            services.Configure<EntityFrameworkDataOptions>(config, configureBinder);
            EntityFrameworkDataOptions options;
            using (var provider = services.BuildServiceProvider(true))
            {
                options = provider.GetRequiredService<IOptions<EntityFrameworkDataOptions>>().Value;
            }

            return services.AddEntityFrameworkDataService<T>(options, optionsAction, contextLifetime, optionsLifetime, name);
        }

        private static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            EntityFrameworkDataOptions options,
            Action<DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime,
            ServiceLifetime optionsLifetime,
            string name)
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
            services.AddDataServices(contextLifetime, name);
            return services;
        }

        private static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            EntityFrameworkDataOptions options,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime,
            ServiceLifetime optionsLifetime,
            string name)
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
            services.AddDataServices(contextLifetime, name);
            return services;
        }

        private static IServiceCollection AddDataServices(
            this IServiceCollection services,
            ServiceLifetime contextLifetime,
            string name)
        {
            using var sp = services.BuildServiceProvider(true);
            var context = sp.GetRequiredService<DbContext>();
            var service = new EntityFrameworkDataService(context, name);
            switch (contextLifetime)
            {
                case Scoped:
                    services.AddScoped<IDataService>(_ => service);
                    services.AddScoped<IDataQueryService>(_ => service);
                    services.AddScoped<IDataCommandService>(_ => service);
                    break;
                case Singleton:
                    services.AddSingleton<IDataService>(service);
                    services.AddSingleton<IDataQueryService>(service);
                    services.AddSingleton<IDataCommandService>(service);
                    break;
                case Transient:
                    services.AddTransient<IDataService>(_ => service);
                    services.AddTransient<IDataQueryService>(_ => service);
                    services.AddTransient<IDataCommandService>(_ => service);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(contextLifetime), contextLifetime, null);
            }

            return services;
        }
    }
}
