namespace Services
{
    using System;
    using Amazon.SimpleEmail;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using static System.String;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAmazonEmailService(
            this IServiceCollection services,
            IConfiguration? configuration)
        {
            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection(nameof(AmazonEmailOptions));
            if (!section.Exists())
            {
                throw new ArgumentException(
                    message: $"{nameof(AmazonEmailOptions)} section doesn't exist",
                    paramName: nameof(configuration));
            }

            services.Configure<AmazonEmailOptions>(section);
            var options = section.Get<AmazonEmailOptions>();
            if (options == default ||
                IsNullOrEmpty(options.AccessKeyId) ||
                IsNullOrEmpty(options.SecretAccessKey))
            {
                throw new ArgumentException(
                    message: $"{nameof(AmazonEmailOptions)} section is invalid",
                    paramName: nameof(configuration));
            }

            services.AddTransient<IAmazonSimpleEmailService, AmazonSimpleEmailServiceClient>(
                implementationFactory: sp => new AmazonSimpleEmailServiceClient(
                    awsAccessKeyId: options.AccessKeyId,
                    awsSecretAccessKey: options.SecretAccessKey));
            services.AddTransient<IEmailService, AmazonEmailService>();
            return services;
        }
    }
}
