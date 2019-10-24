namespace Services
{
    using System;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SendGrid;
    using static System.String;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSendGridEmailService(
            this IServiceCollection services,
            IConfiguration? configuration)
        {
            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection(nameof(SendGridEmailOptions));
            if (!section.Exists())
            {
                throw new ArgumentException(
                    message: $"{nameof(SendGridEmailOptions)} section doesn't exist",
                    paramName: nameof(configuration));
            }

            services.Configure<SendGridClientOptions>(section);
            var options = section.Get<SendGridEmailOptions>();
            if (options == default ||
                IsNullOrEmpty(options.ApiKey))
            {
                throw new ArgumentException(
                    message: $"{nameof(SendGridEmailOptions)} section is invalid",
                    paramName: nameof(configuration));
            }

            services.AddTransient<ISendGridClient, SendGridClient>(
                implementationFactory: sp => new SendGridClient(
                    apiKey: options.ApiKey));
            services.AddTransient<IEmailService, SendGridEmailService>();
            return services;
        }
    }
}
