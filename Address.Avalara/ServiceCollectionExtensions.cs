namespace Services
{
    using System;
    using System.Net.Http.Headers;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using static System.Net.Mime.MediaTypeNames.Application;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAvalaraAddressService(
            this IServiceCollection services,
            IConfiguration? configuration)
        {
            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection(nameof(AvalaraAddressOptions));
            if (!section.Exists())
            {
                throw new Exception($"{nameof(AvalaraAddressOptions)} section doesn't exist");
            }

            services.Configure<AvalaraAddressOptions>(section);
            var avalaraAddressOptions = section.Get<AvalaraAddressOptions>();
            if (avalaraAddressOptions == default ||
                string.IsNullOrEmpty(avalaraAddressOptions.LicenseKey) ||
                string.IsNullOrEmpty(avalaraAddressOptions.BaseAddress))
            {
                throw new Exception($"{nameof(AvalaraAddressOptions)} section is invalid");
            }

            services.AddHttpClient(
                name: nameof(AvalaraAddressService),
                configureClient: httpClient =>
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        scheme: "Basic",
                        parameter: avalaraAddressOptions.LicenseKey);
                    httpClient.BaseAddress = new Uri(avalaraAddressOptions.BaseAddress);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Json));
                });
            services.AddTransient<IAddressService, AvalaraAddressService>();
            return services;
        }
    }
}
