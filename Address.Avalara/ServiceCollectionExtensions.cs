namespace Services
{
    using System;
    using System.Net.Http.Headers;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using static System.Net.Mime.MediaTypeNames.Application;
    using static System.String;

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
                throw new ArgumentException(
                    message: $"{nameof(AvalaraAddressOptions)} section doesn't exist",
                    paramName: nameof(configuration));
            }

            services.Configure<AvalaraAddressOptions>(section);
            var options = section.Get<AvalaraAddressOptions>();
            if (options == default ||
                IsNullOrEmpty(options.LicenseKey) ||
                IsNullOrEmpty(options.BaseAddress))
            {
                throw new ArgumentException(
                    message: $"{nameof(AvalaraAddressOptions)} section is invalid",
                    paramName: nameof(configuration));
            }

            services.AddHttpClient(
                name: nameof(AvalaraAddressService),
                configureClient: httpClient =>
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        scheme: "Basic",
                        parameter: options.LicenseKey);
                    httpClient.BaseAddress = new Uri(options.BaseAddress);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Json));
                });
            services.AddTransient<IAddressService, AvalaraAddressService>();
            return services;
        }
    }
}
