namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Configuration;
    using JetBrains.Annotations;
    using NHibernate;
    using NHibernate.Cfg;
    using Services;
    using static ServiceDescriptor;
    using static Services.Constants.ExceptionMessages;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by "NHibernate" and configured using <paramref name="configureOptions"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="configureOptions">The action to perform on the bound <see cref="NHibernateDataOptions"/>.</param>
        /// <param name="configuration">The <see cref="Configuration"/>.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" /> or <paramref name="configureOptions"/> is <see langword="null" /> or <paramref name="configuration"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="ISessionFactory"/> instance identified by "NHibernate" has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddNHibernate(
            this IServiceCollection services,
            Action<NHibernateDataOptions> configureOptions,
            Configuration configuration)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return services.Configure(nameof(NHibernate), configureOptions).AddNHibernate(nameof(NHibernate), configuration);
        }

        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by "MongoDB" and configured using <paramref name="config"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="config">The <see cref="IConfigurationSection"/> of the <see cref="NHibernateDataOptions"/> instance.</param>
        /// <param name="configuration">The <see cref="Configuration"/>.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" /> or <paramref name="config"/> is <see langword="null" /> or <paramref name="configuration"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="ISessionFactory"/> instance identified by "NHibernate" has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddNHibernate(
            this IServiceCollection services,
            IConfigurationSection config,
            Configuration configuration)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return services.Configure<NHibernateDataOptions>(nameof(NHibernate), config).AddNHibernate(nameof(NHibernate), configuration);
        }

        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by "MongoDB" and configured using <paramref name="config"/> and <paramref name="configureBinder"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="config">The <see cref="IConfigurationSection"/> of the <see cref="NHibernateDataOptions"/> instance.</param>
        /// <param name="configureBinder">The action to perform on the <see cref="BinderOptions"/> of the <see cref="ConfigurationBinder"/>.</param>
        /// <param name="configuration">The <see cref="Configuration"/>.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" /> or <paramref name="config"/> is <see langword="null" /> or <paramref name="configureBinder"/> is <see langword="null" /> or <paramref name="configuration"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="ISessionFactory"/> instance identified by "NHibernate" has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddNHibernate(
            this IServiceCollection services,
            IConfigurationSection config,
            Action<BinderOptions> configureBinder,
            Configuration configuration)
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

            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return services.Configure<NHibernateDataOptions>(nameof(NHibernate), config, configureBinder).AddNHibernate(nameof(NHibernate), configuration);
        }

        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by <paramref name="name"/> and configured using <paramref name="configureOptions"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="name">The name of the <see cref="NHibernateDataOptions"/> instance.</param>
        /// <param name="configureOptions">The action to perform on the bound <see cref="NHibernateDataOptions"/>.</param>
        /// <param name="configuration">The <see cref="Configuration"/>.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" /> or <paramref name="name"/> is <see langword="null" /> or <paramref name="configureOptions"/> is <see langword="null" /> or <paramref name="configuration"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="ISessionFactory"/> instance identified by <paramref name="name"/> has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddNHibernate(
            this IServiceCollection services,
            string name,
            Action<NHibernateDataOptions> configureOptions,
            Configuration configuration)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return services.Configure(name, configureOptions).AddNHibernate(name, configuration);
        }

        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by <paramref name="name"/> and configured using <paramref name="config"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="name">The name of the <see cref="NHibernateDataOptions"/> instance.</param>
        /// <param name="config">The <see cref="IConfigurationSection"/> of the <see cref="NHibernateDataOptions"/> instance.</param>
        /// <param name="configuration">The <see cref="Configuration"/>.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" /> or <paramref name="name"/> is <see langword="null" /> or <paramref name="config"/> is <see langword="null" /> or <paramref name="configuration"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="ISessionFactory"/> instance identified by <paramref name="name"/> has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddNHibernate(
            this IServiceCollection services,
            string name,
            IConfigurationSection config,
            Configuration configuration)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return services.Configure<NHibernateDataOptions>(name, config).AddNHibernate(name, configuration);
        }

        /// <summary>
        /// Adds scoped <see cref="IDataCommandService"/> and <see cref="IDataQueryService"/> services
        /// (identified by <paramref name="name"/> and configured using <paramref name="config"/> and <paramref name="configureBinder"/>)
        /// to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="name">The name of the <see cref="NHibernateDataOptions"/> instance.</param>
        /// <param name="config">The <see cref="IConfigurationSection"/> of the <see cref="NHibernateDataOptions"/> instance.</param>
        /// <param name="configureBinder">The action to perform on the <see cref="BinderOptions"/> of the <see cref="ConfigurationBinder"/>.</param>
        /// <param name="configuration">The <see cref="Configuration"/>.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" /> or <paramref name="name"/> is <see langword="null" /> or <paramref name="config"/> is <see langword="null" /> or <paramref name="configureBinder"/> is <see langword="null" /> or <paramref name="configuration"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">An <see cref="ISessionFactory"/> instance identified by <paramref name="name"/> has already been added to <paramref name="services"/>.</exception>
        public static IServiceCollection AddNHibernate(
            this IServiceCollection services,
            string name,
            IConfigurationSection config,
            Action<BinderOptions> configureBinder,
            Configuration configuration)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return services.Configure<NHibernateDataOptions>(name, config, configureBinder).AddNHibernate(name, configuration);
        }

        private static IServiceCollection AddNHibernate(
            this IServiceCollection services,
            string name,
            Configuration configuration)
        {
            var factory = configuration.BuildSessionFactory();
            services.AddScoped(provider => provider.GetNHibernateDataService(name));
            services.AddScoped(provider => provider.GetNHibernateDataService<IDataCommandService>(name));
            services.AddScoped(provider => provider.GetNHibernateDataService<IDataQueryService>(name));
            var item = services.SingleOrDefault(x => x.ServiceType == typeof(IDictionary<string, ISessionFactory>));
            if (item?.ImplementationInstance is IDictionary<string, ISessionFactory> factories)
            {
                if (factories.ContainsKey(name))
                {
                    throw new ArgumentException(FactoryAlreadyRegistered(name), nameof(name));
                }

                factories.Add(name, factory);
            }
            else
            {
                factories = new Dictionary<string, ISessionFactory>
                {
                    [name] = factory
                };
                item = Singleton(factories);
                services.Add(item);
            }

            return services;
        }
    }
}
