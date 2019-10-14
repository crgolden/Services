namespace Services
{
    using System;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SmartyStreets;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSmartyStreetsAddressService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(SmartyStreetsAddressOptions));
            if (!section.Exists())
            {
                throw new Exception("SmartyStreetsAddressOptions section doesn't exist");
            }

            services.Configure<SmartyStreetsAddressOptions>(section);
            var smartyStreetsAddressOptions = section.Get<SmartyStreetsAddressOptions>();
            if (smartyStreetsAddressOptions == default ||
                string.IsNullOrEmpty(smartyStreetsAddressOptions.AuthId) ||
                string.IsNullOrEmpty(smartyStreetsAddressOptions.AuthToken))
            {
                throw new Exception("SmartyStreetsAddressOptions section is invalid");
            }

            var clientBuilder = new ClientBuilder(
                authId: smartyStreetsAddressOptions.AuthId,
                authToken: smartyStreetsAddressOptions.AuthToken);
            services.AddTransient<IClient<SmartyStreets.USStreetApi.Lookup>>(
                implementationFactory: sp => clientBuilder.BuildUsStreetApiClient());
            services.AddTransient<IClient<SmartyStreets.InternationalStreetApi.Lookup>>(
                implementationFactory: sp => clientBuilder.BuildInternationalStreetApiClient());
            services.AddTransient<IAddressService, SmartyStreetsAddressService>();
            return services;
        }
    }
}
