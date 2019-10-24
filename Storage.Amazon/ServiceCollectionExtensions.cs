namespace Services
{
    using System;
    using Amazon.S3;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using static System.String;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAmazonStorageService(
            this IServiceCollection services,
            IConfiguration? configuration)
        {
            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection(nameof(AmazonStorageOptions));
            if (!section.Exists())
            {
                throw new ArgumentException(
                    message: $"{nameof(AmazonStorageOptions)} section doesn't exist",
                    paramName: nameof(configuration));
            }

            services.Configure<AmazonStorageOptions>(section);
            var options = section.Get<AmazonStorageOptions>();
            if (options == default ||
                IsNullOrEmpty(options.AccessKeyId) ||
                IsNullOrEmpty(options.SecretAccessKey))
            {
                throw new ArgumentException(
                    message: $"{nameof(AmazonStorageOptions)} section is invalid",
                    paramName: nameof(configuration));
            }

            services.AddTransient<IAmazonS3, AmazonS3Client>(
                implementationFactory: sp => new AmazonS3Client(
                    awsAccessKeyId: options.AccessKeyId,
                    awsSecretAccessKey: options.SecretAccessKey));
            services.AddTransient<IStorageService, AmazonStorageService>();
            return services;
        }
    }
}
