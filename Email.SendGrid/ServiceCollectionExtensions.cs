namespace Services
{
    using System;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SendGrid;

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
                throw new Exception($"{nameof(SendGridEmailOptions)} section doesn't exist");
            }

            services.Configure<SendGridClientOptions>(section);
            var sendGridEmailOptions = section.Get<SendGridEmailOptions>();
            if (sendGridEmailOptions == default ||
                string.IsNullOrEmpty(sendGridEmailOptions.ApiKey))
            {
                throw new Exception($"{nameof(SendGridEmailOptions)} section is invalid");
            }

            services.AddTransient<ISendGridClient, SendGridClient>(
                implementationFactory: sp => new SendGridClient(
                    apiKey: sendGridEmailOptions.ApiKey));
            services.AddTransient<IEmailService, SendGridEmailService>();
            return services;
        }
    }
}
