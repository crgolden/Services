namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Azure.ServiceBus;
    using Azure.ServiceBus.Core;
    using Configuration;
    using JetBrains.Annotations;
    using Options;
    using Services;
    using Services.Options;
    using static Azure.ServiceBus.TransportType;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds a hosted <see cref="EmailQueueClientService"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEmailQueueClientService(
            this IServiceCollection services,
            Action<EmailQueueClientOptions> configureOptions)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == default)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddSingleton<IValidateOptions<EmailQueueClientOptions>, ValidateOptions>();
            services.Configure(configureOptions);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<EmailQueueClientOptions>>().Value;
                return services.AddEmailQueueClientService(options);
            }
        }

        /// <summary>Adds a scoped <see cref="EmailQueueClientService"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEmailQueueClientService(
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

            services.AddSingleton<IValidateOptions<EmailQueueClientOptions>, ValidateOptions>();
            services.Configure<EmailQueueClientOptions>(config);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<EmailQueueClientOptions>>().Value;
                return services.AddEmailQueueClientService(options);
            }
        }

        /// <summary>Adds a scoped <see cref="EmailQueueClientService"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddEmailQueueClientService(
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

            services.AddSingleton<IValidateOptions<EmailQueueClientOptions>, ValidateOptions>();
            services.Configure<EmailQueueClientOptions>(config, configureBinder);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<EmailQueueClientOptions>>().Value;
                return services.AddEmailQueueClientService(options);
            }
        }

        private static IServiceCollection AddEmailQueueClientService(this IServiceCollection services, EmailQueueClientOptions options)
        {
            services.AddSingleton<IReceiverClient>(_ =>
            {
                var builder = new ServiceBusConnectionStringBuilder(
                    endpoint: options.Endpoint,
                    entityPath: options.EmailQueueName,
                    sharedAccessKeyName: options.SharedAccessKeyName,
                    sharedAccessKey: options.PrimaryKey ?? options.SecondaryKey,
                    transportType: Amqp);
                return new QueueClient(builder);
            });
            services.AddHostedService<EmailQueueClientService>();
            return services;
        }
    }
}
