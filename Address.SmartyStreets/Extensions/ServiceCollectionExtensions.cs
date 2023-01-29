namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Common.Services;
    using Configuration;
    using JetBrains.Annotations;
    using Options;
    using Services;
    using SmartyStreets;
    using static System.String;
    using InternationalLookup = SmartyStreets.InternationalStreetApi.Lookup;
    using UsLookup = SmartyStreets.USStreetApi.Lookup;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds a <see cref="SmartyStreetsAddressService"/> to <paramref name="services"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The config.</param>
        /// <param name="name">The name.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddSmartyStreetsAddressService(
            this IServiceCollection services,
            Action<SmartyStreetsAddressOptions> configureOptions,
            string name = nameof(SmartyStreetsAddressService))
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == default)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            services.AddSingleton<IValidateOptions<SmartyStreetsAddressOptions>, ValidateSmartyStreetsAddressOptions>();
            services.Configure(configureOptions);
            using var provider = services.BuildServiceProvider(true);
            var options = provider.GetRequiredService<IOptions<SmartyStreetsAddressOptions>>().Value;
            return services.AddSmartyStreetsAddressService(options, name);
        }

        /// <summary>Adds a <see cref="SmartyStreetsAddressService"/> to <paramref name="services"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="name">The name.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddSmartyStreetsAddressService(
            this IServiceCollection services,
            IConfigurationSection config,
            string name = nameof(SmartyStreetsAddressService))
        {
            if (services == default)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == default)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            services.AddSingleton<IValidateOptions<SmartyStreetsAddressOptions>, ValidateSmartyStreetsAddressOptions>();
            services.Configure<SmartyStreetsAddressOptions>(config);
            using var provider = services.BuildServiceProvider(true);
            var options = provider.GetRequiredService<IOptions<SmartyStreetsAddressOptions>>().Value;
            return services.AddSmartyStreetsAddressService(options, name);
        }

        /// <summary>Adds a <see cref="SmartyStreetsAddressService"/> to <paramref name="services"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="configureBinder">The configure binder.</param>
        /// <param name="name">The name.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="configureBinder"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddSmartyStreetsAddressService(
            this IServiceCollection services,
            IConfigurationSection config,
            Action<BinderOptions> configureBinder,
            string name = nameof(SmartyStreetsAddressService))
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

            services.AddSingleton<IValidateOptions<SmartyStreetsAddressOptions>, ValidateSmartyStreetsAddressOptions>();
            services.Configure<SmartyStreetsAddressOptions>(config, configureBinder);
            using var provider = services.BuildServiceProvider(true);
            var options = provider.GetRequiredService<IOptions<SmartyStreetsAddressOptions>>().Value;
            return services.AddSmartyStreetsAddressService(options, name);
        }

        private static IServiceCollection AddSmartyStreetsAddressService(
            this IServiceCollection services,
            SmartyStreetsAddressOptions options,
            string name)
        {
            var clientBuilder = new ClientBuilder(options.AuthId, options.AuthToken);
            services.AddSingleton<IClient<UsLookup>>(_ => clientBuilder.BuildUsStreetApiClient());
            services.AddSingleton<IClient<InternationalLookup>>(_ => clientBuilder.BuildInternationalStreetApiClient());
            services.AddSingleton<IAddressService>(sp =>
            {
                var usClient = sp.GetRequiredService<IClient<UsLookup>>();
                var internationalClient = sp.GetRequiredService<IClient<InternationalLookup>>();
                return new SmartyStreetsAddressService(usClient, internationalClient, name);
            });
            return services;
        }
    }
}
