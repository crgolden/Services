namespace Services.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;
    using Options;
    using Services;
    using static System.String;
    using static Microsoft.Extensions.DependencyInjection.ServiceDescriptor;
    using static Constants.ExceptionMessages;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by "MongoDB" and configured using <paramref name="configureOptions"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="configureOptions">The action to perform on the bound <see cref="MongoDataOptions"/>.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" /> or <paramref name="configureOptions"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="IMongoClient"/> instance identified by "MongoDB" has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddMongo(
            this IServiceCollection services,
            Action<MongoDataOptions> configureOptions)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.Configure(nameof(MongoDB), configureOptions).AddMongo(nameof(MongoDB));
        }

        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by "MongoDB" and configured using <paramref name="config"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="config">The <see cref="IConfigurationSection"/> of the <see cref="MongoDataOptions"/> instance.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" /> or <paramref name="config"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="IMongoClient"/> instance identified by "MongoDB" has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddMongo(
            this IServiceCollection services,
            IConfigurationSection config)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.Configure<MongoDataOptions>(nameof(MongoDB), config).AddMongo(nameof(MongoDB));
        }

        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by "MongoDB" and configured using <paramref name="config"/> and <paramref name="configureBinder"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="config">The <see cref="IConfigurationSection"/> of the <see cref="MongoDataOptions"/> instance.</param>
        /// <param name="configureBinder">The action to perform on the <see cref="BinderOptions"/> of the <see cref="ConfigurationBinder"/>.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" /> or <paramref name="config"/> is <see langword="null" /> or <paramref name="configureBinder"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="IMongoClient"/> instance identified by "MongoDB" has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddMongo(
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

            return services.Configure<MongoDataOptions>(nameof(MongoDB), config, configureBinder).AddMongo(nameof(MongoDB));
        }

        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by <paramref name="name"/> and configured using <paramref name="configureOptions"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="name">The name of the <see cref="MongoDataOptions"/> instance.</param>
        /// <param name="configureOptions">The action to perform on the bound <see cref="MongoDataOptions"/>.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" /> or <paramref name="name"/> is <see langword="null" /> or <paramref name="configureOptions"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="IMongoClient"/> instance identified by <paramref name="name"/> has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddMongo(
            this IServiceCollection services,
            string name,
            Action<MongoDataOptions> configureOptions)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.Configure(name, configureOptions).AddMongo(name);
        }

        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by <paramref name="name"/> and configured using <paramref name="config"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="name">The name of the <see cref="MongoDataOptions"/> instance.</param>
        /// <param name="config">The <see cref="IConfigurationSection"/> of the <see cref="MongoDataOptions"/> instance.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" /> or <paramref name="name"/> is <see langword="null" /> or <paramref name="config"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="IMongoClient"/> instance identified by <paramref name="name"/> has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddMongo(
            this IServiceCollection services,
            string name,
            IConfigurationSection config)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.Configure<MongoDataOptions>(name, config).AddMongo(name);
        }

        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by <paramref name="name"/> and configured using <paramref name="config"/> and <paramref name="configureBinder"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="name">The name of the <see cref="MongoDataOptions"/> instance.</param>
        /// <param name="config">The <see cref="IConfigurationSection"/> of the <see cref="MongoDataOptions"/> instance.</param>
        /// <param name="configureBinder">The action to perform on the <see cref="BinderOptions"/> of the <see cref="ConfigurationBinder"/>.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" /> or <paramref name="name"/> is <see langword="null" /> or <paramref name="config"/> is <see langword="null" /> or <paramref name="configureBinder"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="IMongoClient"/> instance identified by <paramref name="name"/> has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddMongo(
            this IServiceCollection services,
            string name,
            IConfigurationSection config,
            Action<BinderOptions> configureBinder)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.Configure<MongoDataOptions>(name, config, configureBinder).AddMongo(name);
        }

        private static IServiceCollection AddMongo(this IServiceCollection services, string name)
        {
            if (services.All(x => x.ImplementationType != typeof(ValidateOptions)))
            {
                services.AddSingleton<IValidateOptions<MongoDataOptions>, ValidateOptions>();
            }

            if (services.All(x => x.ImplementationType != typeof(ConfigureOptions)))
            {
                services.AddSingleton<IConfigureOptions<MongoDataOptions>, ConfigureOptions>();
            }

            MongoDataOptions options;
            using (var provider = services.BuildServiceProvider(true))
            {
                var monitor = provider.GetRequiredService<IOptionsMonitor<MongoDataOptions>>();
                options = monitor.Get(name);
            }

            IMongoClient client;
            if (options.MongoClientSettings != default)
            {
                client = new MongoClient(options.MongoClientSettings);
            }
            else if (options.MongoUrl != default)
            {
                client = new MongoClient(options.MongoUrl);
            }
            else if (!IsNullOrWhiteSpace(options.ConnectionString))
            {
                client = new MongoClient(options.ConnectionString);
            }
            else
            {
                client = new MongoClient();
            }

            services.AddScoped(provider => provider.GetMongoDataService(name));
            services.AddScoped(provider => provider.GetMongoDataService<IDataCommandService>(name));
            services.AddScoped(provider => provider.GetMongoDataService<IDataQueryService>(name));
            var item = services.SingleOrDefault(x => x.ServiceType == typeof(IDictionary<string, IMongoClient>));
            if (item?.ImplementationInstance is IDictionary<string, IMongoClient> clients)
            {
                if (clients.ContainsKey(name))
                {
                    throw new ArgumentException(ClientAlreadyRegistered(name), nameof(name));
                }

                clients.Add(name, client);
            }
            else
            {
                clients = new Dictionary<string, IMongoClient>
                {
                    [name] = client
                };
                item = Singleton(clients);
                services.Add(item);
            }

            return services;
        }
    }
}
