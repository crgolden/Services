namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Services;
    using Configuration;
    using JetBrains.Annotations;
    using MongoDB.Driver;
    using Options;
    using Services;
    using static System.String;
    using static ServiceDescriptor;
    using static Services.Constants.ExceptionMessages;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by <paramref name="name"/> and configured using <paramref name="configureOptions"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="configureOptions">The action to perform on the bound <see cref="MongoDataOptions"/>.</param>
        /// <param name="name">The name of the <see cref="MongoDataOptions"/> instance.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="IMongoClient"/> instance identified by "MongoDB" has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddMongo(
            this IServiceCollection services,
            Action<MongoDataOptions> configureOptions,
            string name = nameof(MongoDataService))
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return services.Configure(name, configureOptions).AddMongo(name);
        }

        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by <paramref name="name"/> and configured using <paramref name="config"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="config">The <see cref="IConfigurationSection"/> of the <see cref="MongoDataOptions"/> instance.</param>
        /// <param name="name">The name of the <see cref="MongoDataOptions"/> instance.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="IMongoClient"/> instance identified by "MongoDB" has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddMongo(
            this IServiceCollection services,
            IConfigurationSection config,
            string name = nameof(MongoDataService))
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return services.Configure<MongoDataOptions>(name, config).AddMongo(name);
        }

        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by <paramref name="name"/> and configured using <paramref name="config"/> and <paramref name="configureBinder"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="config">The <see cref="IConfigurationSection"/> of the <see cref="MongoDataOptions"/> instance.</param>
        /// <param name="configureBinder">The action to perform on the <see cref="BinderOptions"/> of the <see cref="ConfigurationBinder"/>.</param>
        /// <param name="name">The name of the <see cref="MongoDataOptions"/> instance.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="IMongoClient"/> instance identified by "MongoDB" has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddMongo(
            this IServiceCollection services,
            IConfigurationSection config,
            Action<BinderOptions> configureBinder,
            string name = nameof(MongoDataService))
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

            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return services.Configure<MongoDataOptions>(name, config, configureBinder).AddMongo(name);
        }

        private static IServiceCollection AddMongo(this IServiceCollection services, string name)
        {
            if (services.All(x => x.ImplementationType != typeof(ValidateMongoDataOptions)))
            {
                services.AddSingleton<IValidateOptions<MongoDataOptions>, ValidateMongoDataOptions>();
            }

            if (services.All(x => x.ImplementationType != typeof(ConfigureMongoDataOptions)))
            {
                services.AddSingleton<IConfigureOptions<MongoDataOptions>, ConfigureMongoDataOptions>();
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
