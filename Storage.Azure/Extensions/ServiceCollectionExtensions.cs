namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Azure.Storage;
    using Azure.Storage.Auth;
    using Azure.Storage.Blob;
    using Common;
    using Configuration;
    using JetBrains.Annotations;
    using Options;
    using Services;
    using Services.Options;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds a scoped <see cref="AzureStorageService"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The configure binder.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddAzureStorageService(
            this IServiceCollection services,
            Action<AzureStorageOptions> configureOptions)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == default)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddSingleton<IValidateOptions<AzureStorageOptions>, ValidateOptions>();
            services.Configure(configureOptions);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<AzureStorageOptions>>().Value;
                return services.AddAzureStorageService(options);
            }
        }

        /// <summary>Adds a scoped <see cref="AzureStorageService"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddAzureStorageService(
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

            services.AddSingleton<IValidateOptions<AzureStorageOptions>, ValidateOptions>();
            services.Configure<AzureStorageOptions>(config);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<AzureStorageOptions>>().Value;
                return services.AddAzureStorageService(options);
            }
        }

        /// <summary>Adds a scoped <see cref="AzureStorageService"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddAzureStorageService(
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

            services.AddSingleton<IValidateOptions<AzureStorageOptions>, ValidateOptions>();
            services.Configure<AzureStorageOptions>(config, configureBinder);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<AzureStorageOptions>>().Value;
                return services.AddAzureStorageService(options);
            }
        }

        private static IServiceCollection AddAzureStorageService(this IServiceCollection services, AzureStorageOptions options)
        {
            services.AddSingleton(_ =>
            {
                var storageCredentials = new StorageCredentials(
                    accountName: options.AccountName,
                    keyValue: options.AccountKey1 ?? options.AccountKey2);
                var storageAccount = new CloudStorageAccount(
                    storageCredentials: storageCredentials,
                    useHttps: true);
                return storageAccount.CreateCloudBlobClient();
            });
            services.AddSingleton<IStorageService, AzureStorageService>();
            return services;
        }
    }
}
