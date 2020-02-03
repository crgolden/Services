namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Net.Http.Headers;
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
        /// <summary>Adds a <see cref="AvalaraAddressService"/> to <paramref name="services"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddAvalaraAddressService(
            this IServiceCollection services,
            Action<AvalaraAddressOptions> configureOptions)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == default)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddSingleton<IValidateOptions<AvalaraAddressOptions>, ValidateOptions>();
            services.Configure(configureOptions);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<AvalaraAddressOptions>>().Value;
                return services.AddAvalaraAddressService(options);
            }
        }

        /// <summary>Adds a <see cref="AvalaraAddressService"/> to <paramref name="services"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddAvalaraAddressService(
            this IServiceCollection services,
            IConfiguration config)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == default)
            {
                throw new ArgumentNullException(nameof(config));
            }

            services.AddSingleton<IValidateOptions<AvalaraAddressOptions>, ValidateOptions>();
            services.Configure<AvalaraAddressOptions>(config);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<AvalaraAddressOptions>>().Value;
                return services.AddAvalaraAddressService(options);
            }
        }

        /// <summary>Adds a <see cref="AvalaraAddressService"/> to <paramref name="services"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddAvalaraAddressService(
            this IServiceCollection services,
            IConfiguration config,
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

            services.AddSingleton<IValidateOptions<AvalaraAddressOptions>, ValidateOptions>();
            services.Configure<AvalaraAddressOptions>(config, configureBinder);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<AvalaraAddressOptions>>().Value;
                return services.AddAvalaraAddressService(options);
            }
        }

        private static IServiceCollection AddAvalaraAddressService(this IServiceCollection services, AvalaraAddressOptions options)
        {
            services.AddHttpClient(
                nameof(AvalaraAddressService),
                httpClient =>
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", options.LicenseKey);
                    httpClient.BaseAddress = new Uri(options.BaseAddress);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                });
            services.AddSingleton<IAddressService, AvalaraAddressService>();
            return services;
        }
    }
}
