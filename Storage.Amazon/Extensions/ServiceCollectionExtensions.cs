namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Amazon.S3;
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
        /// <summary>Adds a scoped <see cref="AmazonStorageService"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddAmazonStorageService(
            this IServiceCollection services,
            Action<AmazonStorageOptions> configureOptions)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == default)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddSingleton<IValidateOptions<AmazonStorageOptions>, ValidateOptions>();
            services.Configure(configureOptions);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<AmazonStorageOptions>>().Value;
                return services.AddAmazonStorageService(options);
            }
        }

        /// <summary>Adds a scoped <see cref="AmazonStorageService"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddAmazonStorageService(
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

            services.AddSingleton<IValidateOptions<AmazonStorageOptions>, ValidateOptions>();
            services.Configure<AmazonStorageOptions>(config);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<AmazonStorageOptions>>().Value;
                return services.AddAmazonStorageService(options);
            }
        }

        /// <summary>Adds a scoped <see cref="AmazonStorageService"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddAmazonStorageService(
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

            services.AddSingleton<IValidateOptions<AmazonStorageOptions>, ValidateOptions>();
            services.Configure<AmazonStorageOptions>(config, configureBinder);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<AmazonStorageOptions>>().Value;
                return services.AddAmazonStorageService(options);
            }
        }

        private static IServiceCollection AddAmazonStorageService(
            this IServiceCollection services,
            AmazonStorageOptions options)
        {
            services.AddSingleton<IAmazonS3, AmazonS3Client>(_ => new AmazonS3Client(
                awsAccessKeyId: options.AccessKeyId,
                awsSecretAccessKey: options.SecretAccessKey));
            services.AddSingleton<IStorageService, AmazonStorageService>();
            return services;
        }
    }
}
