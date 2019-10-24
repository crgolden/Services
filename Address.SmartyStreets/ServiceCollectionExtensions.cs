namespace Services
{
    using System;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SmartyStreets;
    using static System.String;
    using InternationalLookup = SmartyStreets.InternationalStreetApi.Lookup;
    using UsLookup = SmartyStreets.USStreetApi.Lookup;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSmartyStreetsAddressService(
            this IServiceCollection services,
            IConfiguration? configuration)
        {
            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection(nameof(SmartyStreetsAddressOptions));
            if (!section.Exists())
            {
                throw new ArgumentException(
                    message: $"{nameof(SmartyStreetsAddressOptions)} section doesn't exist",
                    paramName: nameof(configuration));
            }

            services.Configure<SmartyStreetsAddressOptions>(section);
            var options = section.Get<SmartyStreetsAddressOptions>();
            if (options == default ||
                IsNullOrEmpty(options.AuthId) ||
                IsNullOrEmpty(options.AuthToken))
            {
                throw new ArgumentException(
                    message: $"{nameof(SmartyStreetsAddressOptions)} section is invalid",
                    paramName: nameof(configuration));
            }

            var clientBuilder = new ClientBuilder(
                authId: options.AuthId,
                authToken: options.AuthToken);
            services.AddTransient<IClient<UsLookup>>(
                implementationFactory: sp => clientBuilder.BuildUsStreetApiClient());
            services.AddTransient<IClient<InternationalLookup>>(
                implementationFactory: sp => clientBuilder.BuildInternationalStreetApiClient());
            services.AddTransient<IAddressService, SmartyStreetsAddressService>();
            return services;
        }
    }
}
