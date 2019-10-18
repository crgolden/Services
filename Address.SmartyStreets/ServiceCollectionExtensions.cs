namespace Services
{
    using System;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SmartyStreets;
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
                throw new Exception($"{nameof(SmartyStreetsAddressOptions)} section doesn't exist");
            }

            services.Configure<SmartyStreetsAddressOptions>(section);
            var smartyStreetsAddressOptions = section.Get<SmartyStreetsAddressOptions>();
            if (smartyStreetsAddressOptions == default ||
                string.IsNullOrEmpty(smartyStreetsAddressOptions.AuthId) ||
                string.IsNullOrEmpty(smartyStreetsAddressOptions.AuthToken))
            {
                throw new Exception($"{nameof(SmartyStreetsAddressOptions)} section is invalid");
            }

            var clientBuilder = new ClientBuilder(
                authId: smartyStreetsAddressOptions.AuthId,
                authToken: smartyStreetsAddressOptions.AuthToken);
            services.AddTransient<IClient<UsLookup>>(
                implementationFactory: sp => clientBuilder.BuildUsStreetApiClient());
            services.AddTransient<IClient<InternationalLookup>>(
                implementationFactory: sp => clientBuilder.BuildInternationalStreetApiClient());
            services.AddTransient<IAddressService, SmartyStreetsAddressService>();
            return services;
        }
    }
}
