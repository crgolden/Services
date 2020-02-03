namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Common;
    using Configuration;
    using JetBrains.Annotations;
    using Options;
    using Services;
    using Services.Options;
    using SmartyStreets;
    using InternationalLookup = SmartyStreets.InternationalStreetApi.Lookup;
    using UsLookup = SmartyStreets.USStreetApi.Lookup;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds a <see cref="SmartyStreetsAddressService"/> to <paramref name="services"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddSmartyStreetsAddressService(
            this IServiceCollection services,
            Action<SmartyStreetsAddressOptions> configureOptions)
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == default)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddSingleton<IValidateOptions<SmartyStreetsAddressOptions>, ValidateOptions>();
            services.Configure(configureOptions);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<SmartyStreetsAddressOptions>>().Value;
                return services.AddSmartyStreetsAddressService(options);
            }
        }

        /// <summary>Adds a <see cref="SmartyStreetsAddressService"/> to <paramref name="services"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddSmartyStreetsAddressService(
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

            services.AddSingleton<IValidateOptions<SmartyStreetsAddressOptions>, ValidateOptions>();
            services.Configure<SmartyStreetsAddressOptions>(config);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<SmartyStreetsAddressOptions>>().Value;
                return services.AddSmartyStreetsAddressService(options);
            }
        }

        /// <summary>Adds a <see cref="SmartyStreetsAddressService"/> to <paramref name="services"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddSmartyStreetsAddressService(
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

            services.AddSingleton<IValidateOptions<SmartyStreetsAddressOptions>, ValidateOptions>();
            services.Configure<SmartyStreetsAddressOptions>(config, configureBinder);
            using (var provider = services.BuildServiceProvider(true))
            {
                var options = provider.GetRequiredService<IOptions<SmartyStreetsAddressOptions>>().Value;
                return services.AddSmartyStreetsAddressService(options);
            }
        }

        private static IServiceCollection AddSmartyStreetsAddressService(
            this IServiceCollection services,
            SmartyStreetsAddressOptions options)
        {
            var clientBuilder = new ClientBuilder(options.AuthId, options.AuthToken);
            services.AddSingleton<IClient<UsLookup>>(_ => clientBuilder.BuildUsStreetApiClient());
            services.AddSingleton<IClient<InternationalLookup>>(_ => clientBuilder.BuildInternationalStreetApiClient());
            services.AddSingleton<IAddressService, SmartyStreetsAddressService>();
            return services;
        }
    }
}
