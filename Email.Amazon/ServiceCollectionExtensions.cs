namespace Services
{
    using System;
    using Amazon.SimpleEmail;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAmazonEmailService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(AmazonEmailOptions));
            if (!section.Exists())
            {
                throw new Exception("AmazonEmailOptions section doesn't exist");
            }

            services.Configure<AmazonEmailOptions>(section);
            var amazonEmailOptions = section.Get<AmazonEmailOptions>();
            if (amazonEmailOptions == default ||
                string.IsNullOrEmpty(amazonEmailOptions.AccessKeyId) ||
                string.IsNullOrEmpty(amazonEmailOptions.SecretAccessKey))
            {
                throw new Exception("AmazonEmailOptions section is invalid");
            }

            services.AddTransient<IAmazonSimpleEmailService, AmazonSimpleEmailServiceClient>(
                implementationFactory: sp => new AmazonSimpleEmailServiceClient(
                    awsAccessKeyId: amazonEmailOptions.AccessKeyId,
                    awsSecretAccessKey: amazonEmailOptions.SecretAccessKey));
            services.AddTransient<IEmailService, AmazonEmailService>();
            return services;
        }
    }
}
