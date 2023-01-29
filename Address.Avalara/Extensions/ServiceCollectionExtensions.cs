namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Common.Services;
    using Configuration;
    using JetBrains.Annotations;
    using Options;
    using Services;
    using static System.String;

    /// <summary>A class with methods that extend <see cref="IServiceCollection"/>.</summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds a <see cref="AvalaraAddressService"/> to <paramref name="services"/> using the provided <paramref name="configureOptions"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The config.</param>
        /// <param name="name">The name.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="configureOptions"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddAvalaraAddressService(
            this IServiceCollection services,
            Action<AvalaraAddressOptions> configureOptions,
            string name = nameof(AvalaraAddressService))
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

            services.AddSingleton<IValidateOptions<AvalaraAddressOptions>, ValidateAvalaraAddressOptions>();
            services.Configure(configureOptions);
            using var provider = services.BuildServiceProvider(true);
            var options = provider.GetRequiredService<IOptions<AvalaraAddressOptions>>().Value;
            return services.AddAvalaraAddressService(options, name);
        }

        /// <summary>Adds a <see cref="AvalaraAddressService"/> to <paramref name="services"/> using the provided <paramref name="config"/>.</summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The config.</param>
        /// <param name="name">The name.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />
        /// or
        /// <paramref name="config"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        public static IServiceCollection AddAvalaraAddressService(
            this IServiceCollection services,
            IConfiguration config,
            string name = nameof(AvalaraAddressService))
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

            services.AddSingleton<IValidateOptions<AvalaraAddressOptions>, ValidateAvalaraAddressOptions>();
            services.Configure<AvalaraAddressOptions>(config);
            using var provider = services.BuildServiceProvider(true);
            var options = provider.GetRequiredService<IOptions<AvalaraAddressOptions>>().Value;
            return services.AddAvalaraAddressService(options, name);
        }

        /// <summary>Adds a <see cref="AvalaraAddressService"/> to <paramref name="services"/> using the provided <paramref name="config"/> and <paramref name="configureBinder"/>.</summary>
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
        public static IServiceCollection AddAvalaraAddressService(
            this IServiceCollection services,
            IConfiguration config,
            Action<BinderOptions> configureBinder,
            string name = nameof(AvalaraAddressService))
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

            services.AddSingleton<IValidateOptions<AvalaraAddressOptions>, ValidateAvalaraAddressOptions>();
            services.Configure<AvalaraAddressOptions>(config, configureBinder);
            using var provider = services.BuildServiceProvider(true);
            var options = provider.GetRequiredService<IOptions<AvalaraAddressOptions>>().Value;
            return services.AddAvalaraAddressService(options, name);
        }

        private static IServiceCollection AddAvalaraAddressService(
            this IServiceCollection services,
            AvalaraAddressOptions options,
            string name)
        {
            services.AddHttpClient(
                name,
                httpClient =>
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", options.LicenseKey);
                    httpClient.BaseAddress = options.BaseAddress;
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                });
            services.AddSingleton<IAddressService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                return new AvalaraAddressService(httpClientFactory, name);
            });
            return services;
        }
    }
}
