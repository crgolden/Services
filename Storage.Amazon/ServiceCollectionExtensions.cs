namespace Services
{
    using System;
    using Amazon.S3;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

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
                throw new Exception($"{nameof(AmazonStorageOptions)} section doesn't exist");
            }

            services.Configure<AmazonStorageOptions>(section);
            var amazonStorageOptions = section.Get<AmazonStorageOptions>();
            if (amazonStorageOptions == default ||
                string.IsNullOrEmpty(amazonStorageOptions.AccessKeyId) ||
                string.IsNullOrEmpty(amazonStorageOptions.SecretAccessKey))
            {
                throw new Exception($"{nameof(AmazonStorageOptions)} section is invalid");
            }

            services.AddTransient<IAmazonS3, AmazonS3Client>(
                implementationFactory: sp => new AmazonS3Client(
                    awsAccessKeyId: amazonStorageOptions.AccessKeyId,
                    awsSecretAccessKey: amazonStorageOptions.SecretAccessKey));
            services.AddTransient<IStorageService, AmazonStorageService>();
            return services;
        }
    }
}
