namespace Services
{
    using System;
    using Common;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Auth;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using static System.String;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureStorageService(
            this IServiceCollection services,
            IConfiguration? configuration)
        {
            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection(nameof(AzureStorageOptions));
            if (!section.Exists())
            {
                throw new ArgumentException(
                    message: $"{nameof(AzureStorageOptions)} section doesn't exist",
                    paramName: nameof(configuration));
            }

            services.Configure<AzureStorageOptions>(section);
            var options = section.Get<AzureStorageOptions>();
            if (options == default ||
                IsNullOrEmpty(options.AccountName) ||
                (IsNullOrEmpty(options.AccountKey1) && IsNullOrEmpty(options.AccountKey2)))
            {
                throw new ArgumentException(
                    message: $"{nameof(AzureStorageOptions)} section is invalid",
                    paramName: nameof(configuration));
            }

            services.AddTransient(
                implementationFactory: sp =>
                {
                    var storageCredentials = new StorageCredentials(
                        accountName: options.AccountName,
                        keyValue: options.AccountKey1 ?? options.AccountKey2);
                    var storageAccount = new CloudStorageAccount(
                        storageCredentials: storageCredentials,
                        useHttps: true);
                    return storageAccount.CreateCloudBlobClient();
                });
            services.AddTransient<IStorageService, AzureStorageService>();
            return services;
        }
    }
}
